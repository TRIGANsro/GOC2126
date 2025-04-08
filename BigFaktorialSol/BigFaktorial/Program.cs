using BigNumbers;

namespace BigFaktorial
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            int n = 1_000;
            if (args.Length > 0)
            {
                n = int.Parse(args[0]);
            }

            Integer fact = await Factorial(n,16);

            Console.WriteLine(fact);
        }

        public static async Task<Integer> Factorial(int n, int parts)
        {
            Integer[] partResult = new Integer[parts];
            Task[] tasks = new Task[parts];

            int partSize = n / parts;
            int start = 1;
            for (int i = 0; i < parts; i++)
            {
                int end = start + partSize - 1;
                if (i == parts - 1)
                {
                    end = n;
                }
                int index = i;
                int startpart = start;
                int endpart = end;
                tasks[index] = Task.Run(() => partResult[index] = PartFactorial(startpart, endpart));
                start = end + 1;
            }
            await Task.WhenAll(tasks);

            int pocet = parts;

            while(pocet > 1)
            {
                int index = 0;
                Task[] tasks2 = new Task[pocet/2];
                for (int i = 0; i < pocet/2; i++)
                {
                    int index1 = i;
                    int index2 = pocet -1 - i;
                    tasks2[index] = Task.Run(() => partResult[index1] = partResult[index1].Multiple(partResult[index2]));
                    index++;
                }
                await Task.WhenAll(tasks2);
                pocet = pocet / 2;
            }
            

            return partResult[0];//.Multiple(partResult[1]);
        }

        private static Integer PartFactorial(int start, int end)
        {
            Integer result = new Integer(1);
            for (int i = start; i <= end; i++)
            {
                result = result.Multiple(new Integer(i));
                if (i % 1000 == 0)
                {
                    Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " " + i);
                }
            }
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " end");
            return result;
        }
    }
}
