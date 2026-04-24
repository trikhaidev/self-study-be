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
        // var t1 = ExcuteSomeThing(cts.Token);

        // var ta = new Task<string>((title) =>
        // {
        //     while (true)
        //     {
        //         System.Console.WriteLine(title);
        //         Thread.Sleep(1000);
        //     }
        //     return "Done Task";
        // },"Hello something", cts.Token);
        // ta.Start();

        // var task = Task.Run(async () =>
        // {
        //     while (!cts.IsCancellationRequested)
        //     {
        //         System.Console.WriteLine("Hello something");
        //         // Thread.Sleep(1500);
        //         await Task.Delay(1500, cts.Token);
        //     }
        // });

        // var task = new Task(async () =>
        // {
        //     while (!cts.IsCancellationRequested)
        //     {
        //         System.Console.WriteLine("Hello something");
        //         // Thread.Sleep(1500);
        //         await Task.Delay(1500, cts.Token);
        //     }
        // });

        var task = new Task(async () =>
        {
            // ExcuteSomeThing(cts.Token).Wait();
            await ExcuteSomeThing(cts.Token);
        }, cts.Token);
        task.Start();
        while (!cts.IsCancellationRequested)
        {
            System.Console.WriteLine("Doing something else...");
            await Task.Delay(1000);
        }
        await task;
        Console.ReadLine();
        System.Console.WriteLine("Canceling...");
        cts.Cancel();
        System.Console.WriteLine("Canceled");
        Console.ReadLine();
    }
}
