"""Testes de montar_apng. Rodar na raiz do projeto: python -m unittest discover -s ferramentas"""
import struct
import unittest
import zlib

from montar_apng import ASSINATURA_PNG, ler_chunks, montar_apng, montar_chunk


def png_solido(largura, altura, cor):
    """Cria um PNG RGB de uma cor só, para os testes."""
    ihdr = struct.pack(">IIBBBBB", largura, altura, 8, 2, 0, 0, 0)
    linha = b"\x00" + bytes(cor) * largura
    idat = zlib.compress(linha * altura)
    return (ASSINATURA_PNG + montar_chunk(b"IHDR", ihdr) + montar_chunk(b"IDAT", idat)
            + montar_chunk(b"IEND", b""))


class MontarApngTestes(unittest.TestCase):
    def test_tres_quadros_geram_actl_e_sequencia_correta(self):
        quadros = [png_solido(4, 2, (255, 0, 0)), png_solido(4, 2, (0, 255, 0)), png_solido(4, 2, (0, 0, 255))]

        apng = montar_apng(quadros, 50)
        chunks = list(ler_chunks(apng))
        tipos = [tipo for tipo, _ in chunks]

        self.assertEqual(tipos, [b"IHDR", b"acTL", b"fcTL", b"IDAT", b"fcTL", b"fdAT", b"fcTL", b"fdAT", b"IEND"])
        quantidade, repeticoes = struct.unpack(">II", chunks[1][1])
        self.assertEqual((quantidade, repeticoes), (3, 0))
        sequencias = []
        for tipo, conteudo in chunks:
            if tipo in (b"fcTL", b"fdAT"):
                sequencias.append(struct.unpack(">I", conteudo[:4])[0])
        self.assertEqual(sequencias, [0, 1, 2, 3, 4])

    def test_fctl_tem_tamanho_e_atraso(self):
        apng = montar_apng([png_solido(4, 2, (1, 2, 3))], 40)
        fctl = next(conteudo for tipo, conteudo in ler_chunks(apng) if tipo == b"fcTL")

        _, largura, altura, _, _, atraso_num, atraso_den, _, _ = struct.unpack(">IIIIIHHBB", fctl)

        self.assertEqual((largura, altura, atraso_num, atraso_den), (4, 2, 40, 1000))

    def test_crc_dos_chunks_eh_valido(self):
        apng = montar_apng([png_solido(2, 2, (9, 9, 9)), png_solido(2, 2, (8, 8, 8))], 50)
        posicao = 8
        while posicao < len(apng):
            tamanho = struct.unpack(">I", apng[posicao:posicao + 4])[0]
            tipo_e_conteudo = apng[posicao + 4:posicao + 8 + tamanho]
            crc = struct.unpack(">I", apng[posicao + 8 + tamanho:posicao + 12 + tamanho])[0]
            self.assertEqual(crc, zlib.crc32(tipo_e_conteudo) & 0xFFFFFFFF)
            posicao += 12 + tamanho

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


if __name__ == "__main__":
    unittest.main()
