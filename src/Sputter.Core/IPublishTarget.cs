using FluentResults;

namespace Sputter.Core;

public interface IPublishTarget {
	Task<Result> PublishMeasurement(MeasurementResult result);
}
