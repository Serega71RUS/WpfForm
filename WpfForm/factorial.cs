using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace factorial
{
    class factorial
    {
        private static object locker = new object();
        public static int fact(int n)
        {
            int f = 1;
            for (int i = 1; i <= n;  i++)
            {
                f = f * i;
            }
            return f;
        }

        public static int fact(int n, bool m1)
        {
            if (m1 == true)
            {
                int f = 1;
                for (int i = 1; i <= n; i++)
                {
                    f = f * i;
                    Thread.Sleep(1000);
                }
                return f;
            }
            else
            {
                return 0;
            }
        }
    }
}
