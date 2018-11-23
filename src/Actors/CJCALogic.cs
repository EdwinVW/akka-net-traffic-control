namespace Actors
{
    public static class CJCALogic
    {
        /// <summary>
        /// Calculate fine based on the violation.
        /// </summary>
        /// <param name="violationInKmh">The amount of Km/h the driver was speeding.</param>
        /// <returns>The fine.</returns>
        public static decimal CalculateFine(double violationInKmh)
        {
            switch(violationInKmh)
            {
                case 1: return 10;
                case 2: return 14;
                case 3: return 18;
                case 4: return 23;
                case 5: return 29;
                case 6: return 35;
                case 7: return 40;
                case 8: return 47;
                case 9: return 53;
                case 10: return 60;
                case 11: return 82;
                case 12: return 90;
                case 13: return 96;
                case 14: return 103;
                case 15: return 114;
                case 16: return 123;
                case 17: return 133;
                case 18: return 142;
                case 19: return 153;
                case 20: return 164;
                case 21: return 176;
                case 22: return 185;
                case 23: return 197;
                case 24: return 208;
                case 25: return 216;
                case 26: return 227;
                case 27: return 238;
                case 28: return 249;
                case 29: return 261;
                case 30: return 275;
                case 31: return 283;
                case 32: return 295;
                case 33: return 313;
                case 34: return 329;
                case 35: return 338;
                case 36: return 356;
                case 37: return 373;
                case 38: return 384;
                case 39: return 407;
                case 40: return 409;
            }

            return 0;
        }
    }
}