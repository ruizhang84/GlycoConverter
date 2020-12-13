using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PrecursorIonClassLibrary.Charges;

namespace XMLUnitTest
{
    public class SpectrumProcessUnitTest
    {
        [Test]
        public void test1()
        {
            AirPLS airPLS = new AirPLS();
            int length = 10;
            double[,] x = new double[1, length];
            double[,] w = new double[length, length];
            for (int i = 0; i < length; i++)
            {
                x[0, i] = i;
                w[i, i] = 1;
            }
            double[,] z = airPLS.WhittakerSmooth(x, w);
            //for(int i = 0; i < length; i++)
            //{
            //    Console.WriteLine(z[0, i]);
            //}

            z = airPLS.Correction(x);
            for (int i = 0; i < length; i++)
            {
                Console.WriteLine(z[0, i]);
            }

        }
    }
}
