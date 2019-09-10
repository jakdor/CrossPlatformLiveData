# CrossPlatformLiveData
### Android LiveData inspired .NET implementation - lifecycle aware rx streams

[![Build Status](https://travis-ci.com/jakdor/CrossPlatformLiveData.svg?token=9z6ECote9yoBZxM4Xvob&branch=master)](https://travis-ci.com/jakdor/CrossPlatformLiveData)

CrossPlatformLiveData is a C# library inspired by Android Jetpack LiveData, intended for platform independent use with Xamarin and other cross platform frameworks.

CrossPlatformLiveData is a light weight solution to effective implementation of MVVM architecture pattern.

Features:
* Rx stream with UI lifecycle awareness,
* Last value cache - state preservation,
* Everything that you need to implement MVVM pattern

![CrossPlatformLiveData - MVVM pattern](https://raw.github.com/jakdor/CrossPlatformLiveData/tree/master/img/mvvm.png)

Installation
------

- Core/Logic project - add CrossPlatformLiveData nuget,
- Android project - add CrossPlatformLiveData, CrossPlatformLiveData.Android nuggets,
- iOS project - add CrossPlatformLiveData, CrossPlatformLiveData.iOS nuggets

CrossPlatformLiveData.Android provides base UI classes with LifecycleManger linked to lifecycle events:\
**LiveDataActivity, LiveDataAppCompatActivity, LiveDataAppCompatDialogFragment, LiveDataFragment, LiveDataFragmentActivity, LiveDataSupportDialogFragment, LiveDataSupportFragment**

CrossPlatformLiveData.iOS provides base UI classes with LifecycleManger linked to lifecycle events:\
**LiveDataViewController**

For other platforms all you need to do is provide LifecycleManger with lifecyle events when they occur, implementation should be self explanatory.

Usage
------

**Logic/Core:**

Add LiveData fields in ViewModel/Presenter class:
```cs
public ILiveData<string> SampleLiveData { get; } = new LiveData<string>();
```

Post new value to LiveData:
```cs
SampleLiveData.PostValue("Hello World!");
```

**UI:**

Observe LiveData streams with LifecycleManager
```cs
viewModel.SampleLiveData.Observe(LifecycleManager, OnNextSample, e => {//handle error here}));
```

```cs
private void OnNextSample(string newText)
{
	someTextView.Text = newText;
}
```

RxWrapper (Optional)
------

Optionally you can use RxWrapper template class that reduces boilerplate code by bundling value, request status, and exception in single model:

```cs
public ILiveData<RxWrapper<SampleResponse>> NetworkingLiveData { get; } = new LiveData<RxWrapper<SampleResponse>>();
```

```cs
//Notify UI that request is pending
NetworkingLiveData.PostValue(RxWrapper<SampleResponse>.Pending());
//Data fetch has succeded
NetworkingLiveData.PostValue(RxWrapper<SampleResponse>.Ok(model1));
//Error has occured during network call
NetworkingLiveData.PostValue(RxWrapper<SampleResponse>.Error(new Exception("No network")));
```

```cs
private void OnNextNetworkingData(RxWrapper<SampleResponse> response)
{
	switch (response.Status)
	{
		case RxStatus.Ok:
			//Update UI on succes
			break;
		case RxStatus.Pending:
			//Show loading spinner
			break;
		case RxStatus.Error:
			//Display error alert
			break;
	}
}
```
