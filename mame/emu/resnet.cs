using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public class resnet
    {
        public static byte combine_6_weights(double[] tab,int w0,int w1,int w2,int w3,int w4,int w5)
        {
            return (byte)(tab[0] * w0 + tab[1] * w1 + tab[2] * w2 + tab[3] * w3 + tab[4] * w4 + tab[5] * w5 + 0.5);
        }
        public static byte combine_5_weights(double[] tab, int w0, int w1, int w2, int w3, int w4)
        {
            return (byte)(tab[0] * w0 + tab[1] * w1 + tab[2] * w2 + tab[3] * w3 + tab[4] * w4 + 0.5);
        }
        public static double compute_resistor_weights(int minval, int maxval, double scaler, int count_1, int[] resistances_1, double[] weights_1, int pulldown_1, int pullup_1, int count_2, int[] resistances_2, double[] weights_2, int pulldown_2, int pullup_2, int count_3, int[] resistances_3, double[] weights_3, int pulldown_3, int pullup_3)
        {
            int i, j, n;
            int networks_no;
            int[] rescount = new int[3];
            double[][] r = new double[3][]; ;
            double[][] w = new double[3][];
            double[][] ws = new double[3][];
            int[] r_pd = new int[3];
            int[] r_pu = new int[3];
            double[] max_out = new double[3];
            double[][] out1 = new double[3][];
            for (i = 0; i < 3; i++)
            {
                r[i] = new double[18];
                w[i] = new double[18];
                ws[i] = new double[18];
            }
            double scale;
            double max;
            networks_no = 0;
            for (n = 0; n < 3; n++)
            {
                int count, pd, pu;
                int[] resistances;
                double[] weights;
                switch (n)
                {
                    case 0:
                        count = count_1;
                        resistances = resistances_1;
                        weights = weights_1;
                        pd = pulldown_1;
                        pu = pullup_1;
                        break;
                    case 1:
                        count = count_2;
                        resistances = resistances_2;
                        weights = weights_2;
                        pd = pulldown_2;
                        pu = pullup_2;
                        break;
                    case 2:
                    default:
                        count = count_3;
                        resistances = resistances_3;
                        weights = weights_3;
                        pd = pulldown_3;
                        pu = pullup_3;
                        break;
                }
                if (count > 18)
                {

                }
                if (count > 0)
                {
                    rescount[networks_no] = count;
                    for (i = 0; i < count; i++)
                    {
                        r[networks_no][i] = 1.0 * resistances[i];
                    }
                    out1[networks_no] = weights;
                    r_pd[networks_no] = pd;
                    r_pu[networks_no] = pu;
                    networks_no++;
                }
            }
            if (networks_no < 1)
            {

            }
            for (i = 0; i < networks_no; i++)
            {
                double R0, R1, Vout, dst;
                for (n = 0; n < rescount[i]; n++)
                {
                    R0 = (r_pd[i] == 0) ? 1.0 / 1e12 : 1.0 / r_pd[i];
                    R1 = (r_pu[i] == 0) ? 1.0 / 1e12 : 1.0 / r_pu[i];
                    for (j = 0; j < rescount[i]; j++)
                    {
                        if (j == n)
                        {
                            if (r[i][j] != 0.0)
                            {
                                R1 += 1.0 / r[i][j];
                            }
                        }
                        else if (r[i][j] != 0.0)
                        {
                            R0 += 1.0 / r[i][j];
                        }
                    }
                    R0 = 1.0 / R0;
                    R1 = 1.0 / R1;
                    Vout = (maxval - minval) * R0 / (R1 + R0) + minval;
                    dst = (Vout < minval) ? minval : (Vout > maxval) ? maxval : Vout;
                    w[i][n] = dst;
                }
            }
            j = 0;
            max = 0.0;
            for (i = 0; i < networks_no; i++)
            {
                double sum = 0.0;
                for (n = 0; n < rescount[i]; n++)
                {
                    sum += w[i][n];
                }
                max_out[i] = sum;
                if (max < sum)
                {
                    max = sum;
                    j = i;
                }
            }
            if (scaler < 0.0)
            {
                scale = ((double)maxval) / max_out[j];
            }
            else
            {
                scale = scaler;
            }
            for (i = 0; i < networks_no; i++)
            {
                for (n = 0; n < rescount[i]; n++)
                {
                    ws[i][n] = w[i][n] * scale;
                    out1[i][n] = ws[i][n];
                }
            }
            return (scale);
        }
    }
}
