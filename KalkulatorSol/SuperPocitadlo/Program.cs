namespace SuperPocitadlo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 1000; i++)
            {
                Super super = new Super();
                Task t = Task.Factory.StartNew(() => super.Akce());
                tasks.Add(t);
            }
            await Task.WhenAll(tasks);
        }
    }
}
