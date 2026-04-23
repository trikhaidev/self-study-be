namespace SomeThing;

class Program
{
    static int count = 0;
    static async Task<string> ExcuteSomeThing(CancellationToken cancellationToken)
    {
        do
        {
            System.Console.WriteLine("Count: " + count++);
            await Task.Delay(1000, cancellationToken);
        }while(!cancellationToken.IsCancellationRequested);
        return "Done";
    }

    static async Task Main(string[] args)
    {
        var cts = new CancellationTokenSource();
        //cts.CancelAfter(5000);
        var t1 = ExcuteSomeThing(cts.Token);

        var ta = new Task<string>((title) =>
        {
            while (true)
            {
                System.Console.WriteLine(title);
                Thread.Sleep(1000);
            }
            return "Done Task";
        },"Hello something", cts.Token);
        ta.Start();

        Console.ReadLine();
        System.Console.WriteLine("Canceling...");
        cts.Cancel();
        System.Console.WriteLine("Canceled");
        Console.ReadLine();
    }
}
