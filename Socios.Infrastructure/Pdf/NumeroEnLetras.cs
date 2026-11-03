namespace Socios.Infrastructure.Pdf
{
    /// <summary>
    /// Convierte un importe a letras en español, como se escribe en un recibo:
    /// 10000 → "diez mil con 00/100"; 21500,50 → "veintiún mil quinientos con 50/100".
    /// Soporta hasta 999.999.999.999,99. Los centavos se expresan en número (xx/100).
    /// </summary>
    internal static class NumeroEnLetras
    {
        private static readonly string[] HastaTreinta =
        {
            "cero", "uno", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve",
            "diez", "once", "doce", "trece", "catorce", "quince", "dieciséis", "diecisiete", "dieciocho", "diecinueve",
            "veinte", "veintiuno", "veintidós", "veintitrés", "veinticuatro", "veinticinco", "veintiséis", "veintisiete", "veintiocho", "veintinueve"
        };

        private static readonly string[] Decenas =
            { "", "", "", "treinta", "cuarenta", "cincuenta", "sesenta", "setenta", "ochenta", "noventa" };

        private static readonly string[] Centenas =
            { "", "ciento", "doscientos", "trescientos", "cuatrocientos", "quinientos", "seiscientos", "setecientos", "ochocientos", "novecientos" };

        /// <summary>Importe completo: parte entera en letras y centavos como "xx/100".</summary>
        public static string Importe(decimal monto)
        {
            if (monto < 0 || monto >= 1_000_000_000_000m)
                throw new ArgumentOutOfRangeException(nameof(monto), "El importe está fuera del rango soportado.");

            var redondeado = Math.Round(monto, 2, MidpointRounding.AwayFromZero);
            var entero = (long)Math.Floor(redondeado);
            var centavos = (int)((redondeado - entero) * 100);

            return $"{Entero(entero)} con {centavos:00}/100";
        }

        private static string Entero(long n)
        {
            if (n == 0) return "cero";

            var millones = n / 1_000_000;
            var resto = n % 1_000_000;
            var partes = new List<string>();

            if (millones == 1) partes.Add("un millón");
            else if (millones > 1) partes.Add($"{Apocopar(HastaUnMillon(millones))} millones");

            if (resto > 0) partes.Add(HastaUnMillon(resto));

            return string.Join(" ", partes);
        }

        /// <summary>De 1 a 999.999.</summary>
        private static string HastaUnMillon(long n)
        {
            var miles = n / 1000;
            var resto = (int)(n % 1000);
            var partes = new List<string>();

            if (miles == 1) partes.Add("mil");
            else if (miles > 1) partes.Add($"{Apocopar(HastaMil((int)miles))} mil");

            if (resto > 0) partes.Add(HastaMil(resto));

            return string.Join(" ", partes);
        }

        /// <summary>De 1 a 999.</summary>
        private static string HastaMil(int n)
        {
            if (n == 100) return "cien";

            var centena = n / 100;
            var resto = n % 100;
            if (centena == 0) return HastaCien(resto);

            return resto == 0 ? Centenas[centena] : $"{Centenas[centena]} {HastaCien(resto)}";
        }

        /// <summary>De 1 a 99.</summary>
        private static string HastaCien(int n)
        {
            if (n < 30) return HastaTreinta[n];

            var unidad = n % 10;
            return unidad == 0 ? Decenas[n / 10] : $"{Decenas[n / 10]} y {HastaTreinta[unidad]}";
        }

        /// <summary>
        /// Delante de "mil" y "millones", "uno" se acorta: "veintiuno mil" → "veintiún mil",
        /// "treinta y uno mil" → "treinta y un mil", "ciento uno mil" → "ciento un mil".
        /// </summary>
        private static string Apocopar(string texto)
        {
            if (texto.EndsWith("veintiuno")) return texto[..^"veintiuno".Length] + "veintiún";
            if (texto.EndsWith("uno")) return texto[..^"uno".Length] + "un";
            return texto;
        }
    }
}
