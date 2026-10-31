#!/usr/bin/env python3
"""
Re-delimita archivos CSV: los lee delimitados por coma y los reescribe
con punto y coma (o el delimitador que se indique), para que Excel en
configuracion regional en espanol los abra ya separados en columnas.

Parsea el CSV respetando las comillas, asi que los campos que contengan
comas internas (ej: "Av. Corrientes 1234, Piso 2") no rompen las columnas.

Uso:
    python redelimitar_csv.py <origen> [-o SALIDA] [--entrada-delim ,] [--salida-delim ;]

- <origen>          Archivo .csv o carpeta (procesa todos los .csv).
- -o                Archivo o carpeta de salida. Por defecto reescribe
                    en una carpeta '_redelimitado' junto al origen.
- --entrada-delim   Delimitador actual del archivo (por defecto ",").
- --salida-delim    Delimitador deseado (por defecto ";").
- --encoding        Codificacion de lectura (por defecto utf-8-sig).
"""

import argparse
import csv
import sys
from pathlib import Path


def redelimitar(origen: Path, destino: Path, ent_delim: str, sal_delim: str, encoding: str) -> int:
    """Reescribe un CSV cambiando el delimitador. Devuelve la cantidad de filas."""
    destino.parent.mkdir(parents=True, exist_ok=True)

    filas = 0
    with open(origen, "r", newline="", encoding=encoding) as fin, \
         open(destino, "w", newline="", encoding="utf-8-sig") as fout:
        lector = csv.reader(fin, delimiter=ent_delim)
        escritor = csv.writer(fout, delimiter=sal_delim, quoting=csv.QUOTE_MINIMAL)
        for fila in lector:
            escritor.writerow(fila)
            filas += 1

    return filas


def resolver_salida(origen: Path, salida: Path | None, carpeta_default: Path) -> Path:
    """Determina la ruta de salida para un CSV dado."""
    if salida is None:
        return carpeta_default / origen.name
    if salida.is_dir() or salida.suffix == "":
        return salida / origen.name
    return salida


def main() -> None:
    parser = argparse.ArgumentParser(description="Re-delimita archivos CSV (coma -> punto y coma).")
    parser.add_argument("origen", type=Path, help="Archivo .csv o carpeta con archivos .csv")
    parser.add_argument("-o", "--salida", type=Path, default=None, help="Archivo o carpeta de salida")
    parser.add_argument("--entrada-delim", default=",", help='Delimitador actual (por defecto ",")')
    parser.add_argument("--salida-delim", default=";", help='Delimitador deseado (por defecto ";")')
    parser.add_argument("--encoding", default="utf-8-sig", help="Codificacion de lectura (por defecto utf-8-sig)")
    args = parser.parse_args()

    if not args.origen.exists():
        sys.exit(f"No existe la ruta: {args.origen}")

    if args.origen.is_dir():
        archivos = sorted(args.origen.glob("*.csv")) + sorted(args.origen.glob("*.CSV"))
        if not archivos:
            sys.exit(f"No se encontraron archivos .csv en: {args.origen}")
        carpeta_default = args.origen / "_redelimitado"
    else:
        archivos = [args.origen]
        carpeta_default = args.origen.parent / "_redelimitado"

    total = 0
    for origen in archivos:
        destino = resolver_salida(origen, args.salida, carpeta_default)
        # Evita reescribir un archivo sobre si mismo.
        if destino.resolve() == origen.resolve():
            print(f"OMITIDO  {origen.name}: origen y destino son el mismo archivo", file=sys.stderr)
            continue
        try:
            filas = redelimitar(origen, destino, args.entrada_delim, args.salida_delim, args.encoding)
            print(f"OK  {origen.name} -> {destino}  ({filas} filas)")
            total += 1
        except Exception as exc:  # noqa: BLE001
            print(f"ERROR  {origen.name}: {exc}", file=sys.stderr)

    print(f"\nProcesados {total} de {len(archivos)} archivo(s).")


if __name__ == "__main__":
    main()
