using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTC.AdministrationService
{
    public class ValidationConsts
    {
        // base
        public static int SmallInputMaxLength { get; set; } = 100;
        public static int MediumInputMaxLength { get; set; } = 250;
        public static int LargeInputMaxLength { get; set; } = 500;
        public static int MaxInputMaxLength { get; set; } = 1000;
    }
}
