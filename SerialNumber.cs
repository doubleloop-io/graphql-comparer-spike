using System;

namespace GraphQlComparer
{
    public class SerialNumber
    {
        public Int32 Id { get; }
        public String Barcode { get; }
        public Int32 Tolerated { get; }

        public SerialNumber(int id, string barcode, int tolerated)
        {
            Id = id;
            Barcode = barcode;
            Tolerated = tolerated;
        }
    }
}