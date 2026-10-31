#!/usr/bin/env python3
"""
Convierte archivos DBF a CSV.

Uso:
    python dbf_to_csv.py <origen> [-o SALIDA] [-e ENCODING] [-d DELIMITADOR]

- <origen>   Puede ser un archivo .dbf o una carpeta (procesa todos los .dbf).
- -o         Archivo o carpeta de salida. Por defecto, junto al origen con extension .csv.
- -e         Codificacion del DBF (por defecto se autodetecta desde el header;
             valores comunes en sistemas legacy: cp850, cp1252, latin1).
- -d         Delimitador del CSV (por defecto ",").

Requiere:
    pip install dbfread
"""

import argparse
import csv
import sys
from pathlib import Path

try:
    from dbfread import DBF
except ImportError:
    sys.exit(
        "Falta la libreria 'dbfread'. Instalela con:\n"
        "    pip install dbfread"
    )


def convertir(dbf_path: Path, csv_path: Path, encoding: str | None, delimiter: str) -> int:
    """Convierte un DBF a CSV. Devuelve la cantidad de registros escritos."""
    # char_decode_errors='replace' evita que un byte invalido corte el proceso.
    kwargs = {"load": False, "char_decode_errors": "replace"}
    if encoding:
        kwargs["encoding"] = encoding

    tabla = DBF(str(dbf_path), **kwargs)

    csv_path.parent.mkdir(parents=True, exist_ok=True)

    filas = 0
    with open(csv_path, "w", newline="", encoding="utf-8-sig") as f:
        escritor = csv.writer(f, delimiter=delimiter, quoting=csv.QUOTE_MINIMAL)
        escritor.writerow(tabla.field_names)  # encabezado
        for registro in tabla:
            escritor.writerow([registro[campo] for campo in tabla.field_names])
            filas += 1

    return filas


def resolver_salida(dbf_path: Path, salida: Path | None) -> Path:
    """Determina la ruta del CSV de salida para un DBF dado."""
    if salida is None:
        return dbf_path.with_suffix(".csv")
    if salida.is_dir() or salida.suffix == "":
        return salida / (dbf_path.stem + ".csv")
    return salida


def main() -> None:
    parser = argparse.ArgumentParser(description="Convierte archivos DBF a CSV.")
    parser.add_argument("origen", type=Path, help="Archivo .dbf o carpeta con archivos .dbf")
    parser.add_argument("-o", "--salida", type=Path, default=None, help="Archivo o carpeta de salida")
    parser.add_argument("-e", "--encoding", default=None, help="Codificacion del DBF (ej: cp850, cp1252, latin1)")
    parser.add_argument("-d", "--delimitador", default=",", help='Delimitador del CSV (por defecto ",")')
    args = parser.parse_args()

    if not args.origen.exists():
        sys.exit(f"No existe la ruta: {args.origen}")

    if args.origen.is_dir():
        archivos = sorted(args.origen.glob("*.dbf")) + sorted(args.origen.glob("*.DBF"))
        if not archivos:
            sys.exit(f"No se encontraron archivos .dbf en: {args.origen}")
    else:
        archivos = [args.origen]

    total = 0
    for dbf_path in archivos:
        csv_path = resolver_salida(dbf_path, args.salida)
        try:
            filas = convertir(dbf_path, csv_path, args.encoding, args.delimitador)
            print(f"OK  {dbf_path.name} -> {csv_path}  ({filas} registros)")
            total += 1
        except Exception as exc:  # noqa: BLE001
            print(f"ERROR  {dbf_path.name}: {exc}", file=sys.stderr)

    print(f"\nConvertidos {total} de {len(archivos)} archivo(s).")


if __name__ == "__main__":
    main()
