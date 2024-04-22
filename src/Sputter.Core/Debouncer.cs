namespace Sputter.Core;

//sourced from https://gist.github.com/cocowalla/5d181b82b9a986c6761585000901d1b8

public class Debouncer(TimeSpan? waitTime = null) : IDisposable {
	private readonly CancellationTokenSource cts = new();
	private readonly TimeSpan waitTime = waitTime ?? TimeSpan.FromSeconds(3);
	private int counter;

	public void Debounce(Action action) {
		var current = Interlocked.Increment(ref counter);

		Task.Delay(waitTime).ContinueWith(task =>
		{
			// Is this the last task that was queued?
			if (current == counter && !cts.IsCancellationRequested)
				action();
			task.Dispose();
		}, cts.Token);
	}

	public IDisposable? Debounce(Func<IDisposable?> action) {
		var current = Interlocked.Increment(ref counter);

		return Task.Delay(waitTime).ContinueWith(task => {
			// Is this the last task that was queued?
			if (current == counter && !cts.IsCancellationRequested) {
				return action();
			}
			task.Dispose();
			return null;
		}, cts.Token);
	}

	public void Dispose() {
		cts.Cancel();
		GC.SuppressFinalize(this);
	}
}