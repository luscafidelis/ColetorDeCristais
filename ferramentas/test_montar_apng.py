"""Testes de montar_apng.

Rodar na raiz do projeto, de qualquer uma destas formas:
    python -m unittest discover -s ferramentas
    python -m unittest ferramentas.test_montar_apng
"""
import struct
import sys
import tempfile
import unittest
import zlib
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))

from montar_apng import (  # noqa: E402
    ASSINATURA_PNG,
    decodificar_linhas,
    ler_chunks,
    listar_quadros,
    montar_apng,
    montar_chunk,
    retangulo_alterado,
)


def montar_png(ihdr, idat, extras=()):
    """Monta um PNG a partir do IHDR, dos dados comprimidos e de chunks extras."""
    corpo = montar_chunk(b"IHDR", ihdr)
    for tipo, conteudo in extras:
        corpo += montar_chunk(tipo, conteudo)
    return ASSINATURA_PNG + corpo + montar_chunk(b"IDAT", idat) + montar_chunk(b"IEND", b"")


def ihdr_rgb(largura, altura, tipo_cor=2):
    return struct.pack(">IIBBBBB", largura, altura, 8, tipo_cor, 0, 0, 0)


def png_de_pixels(pixels, filtro=0):
    """Cria um PNG RGB 8 bits a partir de uma matriz de tuplas (r, g, b), com o filtro indicado."""
    altura, largura = len(pixels), len(pixels[0])
    linhas = [bytes(c for pixel in linha for c in pixel) for linha in pixels]
    bruto = b"".join(bytes([filtro]) + filtrar(linha, linhas[y - 1] if y else bytes(len(linha)), filtro, 3)
                     for y, linha in enumerate(linhas))
    return montar_png(ihdr_rgb(largura, altura), zlib.compress(bruto))


def filtrar(linha, anterior, filtro, bpp):
    """Aplica um filtro PNG numa linha (implementação independente, só para os testes)."""
    saida = bytearray()
    for i, valor in enumerate(linha):
        a = linha[i - bpp] if i >= bpp else 0
        b = anterior[i]
        c = anterior[i - bpp] if i >= bpp else 0
        if filtro == 0:
            preditor = 0
        elif filtro == 1:
            preditor = a
        elif filtro == 2:
            preditor = b
        elif filtro == 3:
            preditor = (a + b) // 2
        else:
            p = a + b - c
            pa, pb, pc = abs(p - a), abs(p - b), abs(p - c)
            preditor = a if pa <= pb and pa <= pc else (b if pb <= pc else c)
        saida.append((valor - preditor) & 0xFF)
    return bytes(saida)


def png_solido(largura, altura, cor):
    return png_de_pixels([[cor] * largura for _ in range(altura)])


def chunks_de(apng):
    return list(ler_chunks(apng))


class MontarApngTestes(unittest.TestCase):
    def test_tres_quadros_geram_actl_e_sequencia_correta(self):
        quadros = [png_solido(4, 2, (255, 0, 0)), png_solido(4, 2, (0, 255, 0)), png_solido(4, 2, (0, 0, 255))]

        chunks = chunks_de(montar_apng(quadros, 50))
        tipos = [tipo for tipo, _ in chunks]

        self.assertEqual(tipos, [b"IHDR", b"acTL", b"fcTL", b"IDAT", b"fcTL", b"fdAT", b"fcTL", b"fdAT", b"IEND"])
        self.assertEqual(struct.unpack(">II", chunks[1][1]), (3, 0))
        sequencias = [struct.unpack(">I", c[:4])[0] for t, c in chunks if t in (b"fcTL", b"fdAT")]
        self.assertEqual(sequencias, [0, 1, 2, 3, 4])

    def test_fctl_tem_tamanho_e_atraso(self):
        fctl = next(c for t, c in chunks_de(montar_apng([png_solido(4, 2, (1, 2, 3))], 40)) if t == b"fcTL")

        _, largura, altura, x, y, atraso_num, atraso_den, _, _ = struct.unpack(">IIIIIHHBB", fctl)

        self.assertEqual((largura, altura, x, y, atraso_num, atraso_den), (4, 2, 0, 0, 40, 1000))

    def test_crc_dos_chunks_eh_valido(self):
        apng = montar_apng([png_solido(2, 2, (9, 9, 9)), png_solido(2, 2, (8, 8, 8))], 50)
        posicao = 8
        while posicao < len(apng):
            tamanho = struct.unpack(">I", apng[posicao:posicao + 4])[0]
            tipo_e_conteudo = apng[posicao + 4:posicao + 8 + tamanho]
            crc = struct.unpack(">I", apng[posicao + 8 + tamanho:posicao + 12 + tamanho])[0]
            self.assertEqual(crc, zlib.crc32(tipo_e_conteudo) & 0xFFFFFFFF)
            posicao += 12 + tamanho

    # --- Validação de entrada ---

    def test_quadros_de_tamanhos_diferentes_lancam_erro(self):
        with self.assertRaises(ValueError):
            montar_apng([png_solido(4, 2, (0, 0, 0)), png_solido(2, 2, (0, 0, 0))], 50)

    def test_sem_quadros_ou_atraso_invalido_lancam_erro(self):
        with self.assertRaises(ValueError):
            montar_apng([], 50)
        with self.assertRaises(ValueError):
            montar_apng([png_solido(2, 2, (0, 0, 0))], 0)

    def test_arquivo_que_nao_eh_png_lanca_erro(self):
        with self.assertRaises(ValueError):
            montar_apng([b"nao sou png"], 50)

    def test_png_truncado_lanca_value_error(self):
        png = png_solido(4, 2, (1, 1, 1))

        with self.assertRaises(ValueError):
            montar_apng([png[:-6]], 50)
        with self.assertRaises(ValueError):
            montar_apng([png[:12]], 50)

    def test_png_sem_ihdr_ou_sem_idat_lanca_value_error(self):
        sem_ihdr = ASSINATURA_PNG + montar_chunk(b"IDAT", zlib.compress(b"\x00" * 4)) + montar_chunk(b"IEND", b"")
        sem_idat = ASSINATURA_PNG + montar_chunk(b"IHDR", ihdr_rgb(1, 1)) + montar_chunk(b"IEND", b"")

        with self.assertRaises(ValueError):
            montar_apng([sem_ihdr], 50)
        with self.assertRaises(ValueError):
            montar_apng([sem_idat], 50)

    # --- Chunks de paleta e cor ---

    def test_paleta_e_gama_sao_copiadas_antes_dos_quadros(self):
        plte = bytes([255, 0, 0, 0, 255, 0])
        gama = struct.pack(">I", 45455)
        ihdr = ihdr_rgb(2, 1, tipo_cor=3)
        quadros = [montar_png(ihdr, zlib.compress(b"\x00\x00\x01"), [(b"gAMA", gama), (b"PLTE", plte)]),
                   montar_png(ihdr, zlib.compress(b"\x00\x01\x00"), [(b"gAMA", gama), (b"PLTE", plte)])]

        chunks = chunks_de(montar_apng(quadros, 50))
        tipos = [t for t, _ in chunks]

        self.assertLess(tipos.index(b"PLTE"), tipos.index(b"fcTL"))
        self.assertEqual(dict(chunks)[b"PLTE"], plte)
        self.assertEqual(dict(chunks)[b"gAMA"], gama)
        self.assertEqual(tipos.count(b"fdAT"), 1)

    def test_paletas_diferentes_lancam_erro(self):
        ihdr = ihdr_rgb(1, 1, tipo_cor=3)
        quadros = [montar_png(ihdr, zlib.compress(b"\x00\x00"), [(b"PLTE", bytes([1, 2, 3]))]),
                   montar_png(ihdr, zlib.compress(b"\x00\x00"), [(b"PLTE", bytes([4, 5, 6]))])]

        with self.assertRaises(ValueError):
            montar_apng(quadros, 50)

    # --- Recorte da região alterada ---

    def test_quadro_seguinte_guarda_so_o_retangulo_alterado(self):
        base = [[(10, 10, 10)] * 4 for _ in range(3)]
        alterado = [linha[:] for linha in base]
        alterado[1][2] = (200, 0, 0)

        chunks = chunks_de(montar_apng([png_de_pixels(base), png_de_pixels(alterado)], 50))
        segundo_fctl = [c for t, c in chunks if t == b"fcTL"][1]
        fdat = next(c for t, c in chunks if t == b"fdAT")

        _, largura, altura, x, y, _, _, _, _ = struct.unpack(">IIIIIHHBB", segundo_fctl)
        self.assertEqual((largura, altura, x, y), (1, 1, 2, 1))
        self.assertEqual(zlib.decompress(fdat[4:]), b"\x00" + bytes([200, 0, 0]))

    def test_quadros_iguais_viram_recorte_de_um_pixel(self):
        chunks = chunks_de(montar_apng([png_solido(3, 3, (5, 5, 5)), png_solido(3, 3, (5, 5, 5))], 50))
        segundo_fctl = [c for t, c in chunks if t == b"fcTL"][1]

        self.assertEqual(struct.unpack(">IIIIIHHBB", segundo_fctl)[1:5], (1, 1, 0, 0))

    def test_sem_recorte_guarda_quadros_inteiros(self):
        quadros = [png_solido(3, 2, (1, 1, 1)), png_solido(3, 2, (2, 2, 2))]
        chunks = chunks_de(montar_apng(quadros, 50, recortar=False))
        segundo_fctl = [c for t, c in chunks if t == b"fcTL"][1]

        self.assertEqual(struct.unpack(">IIIIIHHBB", segundo_fctl)[1:5], (3, 2, 0, 0))

    def test_retangulo_alterado_cobre_todas_as_diferencas(self):
        antes = [bytes(9), bytes(9), bytes(9)]
        depois = [bytes(9), bytes([0, 0, 0, 0, 0, 0, 7, 0, 0]), bytes([7, 0, 0, 0, 0, 0, 0, 0, 0])]

        self.assertEqual(retangulo_alterado(antes, depois, 3), (0, 1, 3, 2))

    def test_decodificacao_desfaz_todos_os_filtros(self):
        pixels = [[(x * 40, y * 70, (x + y) * 25) for x in range(5)] for y in range(4)]
        esperado = [bytes(c for pixel in linha for c in pixel) for linha in pixels]

        for filtro in range(5):
            with self.subTest(filtro=filtro):
                png = png_de_pixels(pixels, filtro)
                chunks = dict(ler_chunks(png))
                self.assertEqual(decodificar_linhas(chunks[b"IHDR"], chunks[b"IDAT"], 3), esperado)

    # --- Escolha e ordem dos quadros ---

    def test_listar_quadros_usa_ordem_natural_e_ignora_a_saida(self):
        with tempfile.TemporaryDirectory() as pasta:
            for nome in ["q_10.png", "q_2.png", "q_1000.png", "q_999.png", "animacao.png", "notas.txt"]:
                (Path(pasta) / nome).write_bytes(b"")

            quadros = listar_quadros(pasta, Path(pasta) / "animacao.png")

            self.assertEqual([q.name for q in quadros], ["q_2.png", "q_10.png", "q_999.png", "q_1000.png"])


if __name__ == "__main__":
    unittest.main()
