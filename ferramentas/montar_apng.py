"""Junta vários PNGs do mesmo tamanho num PNG animado (APNG), só com a biblioteca padrão do Python.

Uso (na raiz do projeto):
    python ferramentas/montar_apng.py <pasta_com_pngs> <saida.png> <atraso_ms>

Os quadros são lidos em ordem alfabética (ex.: q_000.png, q_001.png, ...) e precisam ter
o mesmo tamanho e formato. A animação repete para sempre.
"""
import struct
import sys
import zlib
from pathlib import Path

ASSINATURA_PNG = b"\x89PNG\r\n\x1a\n"


def ler_chunks(dados):
    """Percorre os chunks de um arquivo PNG, devolvendo (tipo, conteúdo)."""
    if dados[:8] != ASSINATURA_PNG:
        raise ValueError("O arquivo não é um PNG.")
    posicao = 8
    while posicao < len(dados):
        tamanho = struct.unpack(">I", dados[posicao:posicao + 4])[0]
        tipo = dados[posicao + 4:posicao + 8]
        yield tipo, dados[posicao + 8:posicao + 8 + tamanho]
        posicao += 12 + tamanho


def montar_chunk(tipo, conteudo):
    """Monta um chunk PNG com tamanho e CRC."""
    crc = zlib.crc32(tipo + conteudo) & 0xFFFFFFFF
    return struct.pack(">I", len(conteudo)) + tipo + conteudo + struct.pack(">I", crc)


def montar_apng(quadros_png, atraso_ms):
    """Recebe os bytes de cada PNG e devolve os bytes do APNG.

    :param quadros_png: lista de bytes de arquivos PNG, todos com o mesmo IHDR.
    :param atraso_ms: duração de cada quadro, em milissegundos (1 a 65535).
    :raises ValueError: sem quadros, atraso inválido ou quadros de tamanho/formato diferentes.
    """
    if not quadros_png:
        raise ValueError("Nenhum quadro informado.")
    if not 1 <= atraso_ms <= 65535:
        raise ValueError("O atraso precisa estar entre 1 e 65535 ms.")

    ihdr = None
    dados_de_imagem = []
    for indice, png in enumerate(quadros_png):
        idat = b""
        for tipo, conteudo in ler_chunks(png):
            if tipo == b"IHDR":
                if ihdr is None:
                    ihdr = conteudo
                elif conteudo != ihdr:
                    raise ValueError(f"O quadro {indice} tem tamanho ou formato diferente do primeiro.")
            elif tipo == b"IDAT":
                idat += conteudo
        dados_de_imagem.append(idat)

    largura, altura = struct.unpack(">II", ihdr[:8])
    saida = bytearray(ASSINATURA_PNG)
    saida += montar_chunk(b"IHDR", ihdr)
    saida += montar_chunk(b"acTL", struct.pack(">II", len(dados_de_imagem), 0))  # 0 = repete para sempre
    sequencia = 0
    for indice, idat in enumerate(dados_de_imagem):
        controle = struct.pack(">IIIIIHHBB", sequencia, largura, altura, 0, 0, atraso_ms, 1000, 0, 0)
        saida += montar_chunk(b"fcTL", controle)
        sequencia += 1
        if indice == 0:
            saida += montar_chunk(b"IDAT", idat)
        else:
            saida += montar_chunk(b"fdAT", struct.pack(">I", sequencia) + idat)
            sequencia += 1
    saida += montar_chunk(b"IEND", b"")
    return bytes(saida)


def main(argumentos):
    if len(argumentos) != 3:
        print(__doc__)
        return 2
    pasta, caminho_saida, atraso = argumentos
    arquivos = sorted(Path(pasta).glob("*.png"))
    apng = montar_apng([arquivo.read_bytes() for arquivo in arquivos], int(atraso))
    Path(caminho_saida).write_bytes(apng)
    print(f"{len(arquivos)} quadros, {len(apng) / 1024 / 1024:.2f} MB -> {caminho_saida}")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
