"""Junta vários PNGs do mesmo tamanho num PNG animado (APNG), só com a biblioteca padrão do Python.

Uso (na raiz do projeto):
    python ferramentas/montar_apng.py <pasta_com_pngs> <saida.png> <atraso_ms>

Os quadros são lidos em ordem natural do nome (q_2 antes de q_10; q_999 antes de q_1000) e
precisam ter o mesmo tamanho e formato. Se a saída estiver dentro da pasta, ela é ignorada
como quadro. A animação repete para sempre.

Para economizar espaço, cada quadro depois do primeiro guarda só o retângulo que mudou em
relação ao anterior (PNG RGB/RGBA de 8 bits, sem entrelaçamento). Nos outros formatos, os
quadros são guardados inteiros.
"""
import re
import struct
import sys
import zlib
from pathlib import Path

ASSINATURA_PNG = b"\x89PNG\r\n\x1a\n"

# Chunks que precisam acompanhar a imagem (paleta, transparência e cor). Copiados do primeiro quadro.
CHUNKS_COPIADOS = (b"PLTE", b"tRNS", b"gAMA", b"cHRM", b"sRGB", b"iCCP", b"sBIT", b"pHYs")
# Chunks que precisam ser iguais em todos os quadros, porque o APNG só tem um de cada.
CHUNKS_IGUAIS_EM_TODOS = (b"PLTE", b"tRNS")


def ler_chunks(dados):
    """Percorre os chunks de um arquivo PNG, devolvendo (tipo, conteúdo).

    :raises ValueError: se não for PNG ou se algum chunk estiver truncado.
    """
    if dados[:8] != ASSINATURA_PNG:
        raise ValueError("O arquivo não é um PNG.")
    posicao = 8
    while posicao < len(dados):
        if posicao + 12 > len(dados):
            raise ValueError("PNG truncado: cabeçalho de chunk incompleto.")
        tamanho = struct.unpack(">I", dados[posicao:posicao + 4])[0]
        if posicao + 12 + tamanho > len(dados):
            raise ValueError("PNG truncado: chunk maior que o arquivo.")
        tipo = dados[posicao + 4:posicao + 8]
        yield tipo, dados[posicao + 8:posicao + 8 + tamanho]
        posicao += 12 + tamanho


def montar_chunk(tipo, conteudo):
    """Monta um chunk PNG com tamanho e CRC."""
    crc = zlib.crc32(tipo + conteudo) & 0xFFFFFFFF
    return struct.pack(">I", len(conteudo)) + tipo + conteudo + struct.pack(">I", crc)


def ler_png(dados, indice=0):
    """Lê IHDR, dados de imagem (IDAT juntos) e chunks a copiar de um PNG.

    :raises ValueError: se faltar IHDR ou IDAT, ou se o arquivo for inválido.
    """
    ihdr = None
    partes_idat = []
    extras = []
    for tipo, conteudo in ler_chunks(dados):
        if tipo == b"IHDR":
            ihdr = conteudo
        elif tipo == b"IDAT":
            partes_idat.append(conteudo)
        elif tipo in CHUNKS_COPIADOS:
            extras.append((tipo, conteudo))
    if ihdr is None or len(ihdr) != 13:
        raise ValueError(f"O quadro {indice} não tem IHDR válido.")
    if not partes_idat:
        raise ValueError(f"O quadro {indice} não tem dados de imagem (IDAT).")
    return ihdr, b"".join(partes_idat), extras


def _bytes_por_pixel(ihdr):
    """Bytes por pixel para PNG de 8 bits RGB/RGBA sem entrelaçamento; None nos outros formatos."""
    profundidade, tipo_cor, _, _, entrelacado = struct.unpack(">BBBBB", ihdr[8:13])
    if profundidade != 8 or entrelacado != 0:
        return None
    return {2: 3, 6: 4}.get(tipo_cor)


def _paeth(a, b, c):
    p = a + b - c
    pa, pb, pc = abs(p - a), abs(p - b), abs(p - c)
    if pa <= pb and pa <= pc:
        return a
    return b if pb <= pc else c


def decodificar_linhas(ihdr, idat, bpp):
    """Descomprime e desfaz os filtros do PNG, devolvendo uma lista de linhas (bytes) de pixels."""
    largura, altura = struct.unpack(">II", ihdr[:8])
    tamanho_linha = largura * bpp
    bruto = zlib.decompress(idat)
    if len(bruto) < altura * (tamanho_linha + 1):
        raise ValueError("Dados de imagem menores que o esperado.")
    linhas = []
    anterior = bytearray(tamanho_linha)
    for y in range(altura):
        inicio = y * (tamanho_linha + 1)
        filtro = bruto[inicio]
        linha = bytearray(bruto[inicio + 1:inicio + 1 + tamanho_linha])
        if filtro == 1:
            for i in range(bpp, tamanho_linha):
                linha[i] = (linha[i] + linha[i - bpp]) & 0xFF
        elif filtro == 2:
            for i in range(tamanho_linha):
                linha[i] = (linha[i] + anterior[i]) & 0xFF
        elif filtro == 3:
            for i in range(tamanho_linha):
                esquerda = linha[i - bpp] if i >= bpp else 0
                linha[i] = (linha[i] + ((esquerda + anterior[i]) >> 1)) & 0xFF
        elif filtro == 4:
            for i in range(tamanho_linha):
                esquerda = linha[i - bpp] if i >= bpp else 0
                diagonal = anterior[i - bpp] if i >= bpp else 0
                linha[i] = (linha[i] + _paeth(esquerda, anterior[i], diagonal)) & 0xFF
        elif filtro != 0:
            raise ValueError(f"Filtro PNG desconhecido: {filtro}.")
        linhas.append(bytes(linha))
        anterior = linha
    return linhas


def retangulo_alterado(linhas_antes, linhas_depois, bpp):
    """Menor retângulo (x, y, largura, altura) em pixels que contém todas as diferenças.

    Se os quadros forem iguais, devolve um retângulo de 1×1 em (0, 0), porque o APNG
    não aceita quadros vazios.
    """
    linhas_diferentes = [y for y, (a, b) in enumerate(zip(linhas_antes, linhas_depois)) if a != b]
    if not linhas_diferentes:
        return 0, 0, 1, 1
    x_min, x_max = None, None
    for y in linhas_diferentes:
        a, b = linhas_antes[y], linhas_depois[y]
        inicio = next(i for i in range(len(a)) if a[i] != b[i])
        fim = next(i for i in range(len(a) - 1, -1, -1) if a[i] != b[i])
        x_min = inicio if x_min is None else min(x_min, inicio)
        x_max = fim if x_max is None else max(x_max, fim)
    x = x_min // bpp
    largura = x_max // bpp - x + 1
    y = linhas_diferentes[0]
    altura = linhas_diferentes[-1] - y + 1
    return x, y, largura, altura


def codificar_recorte(linhas, x, y, largura, altura, bpp):
    """Comprime um recorte das linhas como dados IDAT (filtro 0 em todas as linhas)."""
    partes = []
    for linha in linhas[y:y + altura]:
        partes.append(b"\x00" + linha[x * bpp:(x + largura) * bpp])
    return zlib.compress(b"".join(partes), 9)


def montar_apng(quadros_png, atraso_ms, recortar=True):
    """Recebe os bytes de cada PNG e devolve os bytes do APNG.

    :param quadros_png: lista de bytes de arquivos PNG, todos com o mesmo IHDR.
    :param atraso_ms: duração de cada quadro, em milissegundos (1 a 65535).
    :param recortar: guarda só a região alterada de cada quadro (quando o formato permite).
    :raises ValueError: sem quadros, atraso inválido, PNG inválido ou quadros de tamanho,
        formato, paleta ou transparência diferentes.
    """
    if not quadros_png:
        raise ValueError("Nenhum quadro informado.")
    if not 1 <= atraso_ms <= 65535:
        raise ValueError("O atraso precisa estar entre 1 e 65535 ms.")

    lidos = [ler_png(png, indice) for indice, png in enumerate(quadros_png)]
    ihdr, _, extras = lidos[0]
    for indice, (outro_ihdr, _, outros_extras) in enumerate(lidos[1:], start=1):
        if outro_ihdr != ihdr:
            raise ValueError(f"O quadro {indice} tem tamanho ou formato diferente do primeiro.")
        for tipo in CHUNKS_IGUAIS_EM_TODOS:
            if [c for t, c in outros_extras if t == tipo] != [c for t, c in extras if t == tipo]:
                raise ValueError(f"O quadro {indice} tem {tipo.decode()} diferente do primeiro.")

    largura, altura = struct.unpack(">II", ihdr[:8])
    bpp = _bytes_por_pixel(ihdr) if recortar else None

    saida = bytearray(ASSINATURA_PNG)
    saida += montar_chunk(b"IHDR", ihdr)
    saida += montar_chunk(b"acTL", struct.pack(">II", len(lidos), 0))  # 0 = repete para sempre
    for tipo, conteudo in extras:
        saida += montar_chunk(tipo, conteudo)

    sequencia = 0
    linhas_anteriores = decodificar_linhas(ihdr, lidos[0][1], bpp) if bpp else None
    for indice, (_, idat, _) in enumerate(lidos):
        x, y, larg, alt = 0, 0, largura, altura
        if bpp and indice > 0:
            linhas = decodificar_linhas(ihdr, idat, bpp)
            x, y, larg, alt = retangulo_alterado(linhas_anteriores, linhas, bpp)
            idat = codificar_recorte(linhas, x, y, larg, alt, bpp)
            linhas_anteriores = linhas
        # dispose_op 0 (mantém o quadro anterior) e blend_op 0 (substitui a região)
        controle = struct.pack(">IIIIIHHBB", sequencia, larg, alt, x, y, atraso_ms, 1000, 0, 0)
        saida += montar_chunk(b"fcTL", controle)
        sequencia += 1
        if indice == 0:
            saida += montar_chunk(b"IDAT", idat)
        else:
            saida += montar_chunk(b"fdAT", struct.pack(">I", sequencia) + idat)
            sequencia += 1
    saida += montar_chunk(b"IEND", b"")
    return bytes(saida)


def chave_natural(caminho):
    """Chave de ordenação que compara os números do nome pelo valor (q_2 antes de q_10)."""
    return [int(parte) if parte.isdigit() else parte.lower() for parte in re.split(r"(\d+)", caminho.name)]


def listar_quadros(pasta, caminho_saida):
    """PNGs da pasta em ordem natural, sem o arquivo de saída (se ele estiver lá)."""
    saida = Path(caminho_saida).resolve()
    arquivos = [arquivo for arquivo in Path(pasta).glob("*.png") if arquivo.resolve() != saida]
    return sorted(arquivos, key=chave_natural)


def main(argumentos):
    if len(argumentos) != 3:
        print(__doc__)
        return 2
    pasta, caminho_saida, atraso = argumentos
    arquivos = listar_quadros(pasta, caminho_saida)
    apng = montar_apng([arquivo.read_bytes() for arquivo in arquivos], int(atraso))
    Path(caminho_saida).write_bytes(apng)
    print(f"{len(arquivos)} quadros, {len(apng) / 1024 / 1024:.2f} MB -> {caminho_saida}")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
