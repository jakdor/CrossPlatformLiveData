# CrossPlatformLiveData
### Android LiveData inspired .NET implementation - lifecycle aware rx streams

[![Build Status](https://travis-ci.com/jakdor/CrossPlatformLiveData.svg?branch=master)](https://travis-ci.com/jakdor/CrossPlatformLiveData)
[![Nuget](https://img.shields.io/nuget/v/CrossPlatformLiveData)](https://www.nuget.org/packages/CrossPlatformLiveData/)

[Medium article about CrossPlatformLiveData](https://medium.com/@jakdor00/crossplatformlivedata-xamarin-lightweight-mvvm-library-280ab2857bec)

CrossPlatformLiveData is a C# library inspired by Android Jetpack LiveData, intended for platform independent use with Xamarin and other cross platform frameworks.

CrossPlatformLiveData is a light weight solution to effective implementation of MVVM architecture pattern.

Features:
* Rx stream with UI lifecycle awareness,
* Last value cache - state preservation,
* Everything that you need to implement MVVM pattern

![CrossPlatformLiveData - MVVM pattern](https://i.imgur.com/JxfRZdM.png)

Installation
------

- [![Nuget](https://img.shields.io/nuget/v/CrossPlatformLiveData)](https://www.nuget.org/packages/CrossPlatformLiveData/) Core/Logic project - add [CrossPlatformLiveData](https://www.nuget.org/packages/CrossPlatformLiveData/) nuget,
- [![Nuget](https://img.shields.io/nuget/v/CrossPlatformLiveData.Android)](https://www.nuget.org/packages/CrossPlatformLiveData.Android/) Android project - add [CrossPlatformLiveData](https://www.nuget.org/packages/CrossPlatformLiveData/), [CrossPlatformLiveData.Android](https://www.nuget.org/packages/CrossPlatformLiveData.Android/) nuggets,
- [![Nuget](https://img.shields.io/nuget/v/CrossPlatformLiveData.iOS)](https://www.nuget.org/packages/CrossPlatformLiveData.iOS/) iOS project - add [CrossPlatformLiveData](https://www.nuget.org/packages/CrossPlatformLiveData/), [CrossPlatformLiveData.iOS](https://www.nuget.org/packages/CrossPlatformLiveData.iOS/) nuggets

[CrossPlatformLiveData.Android](https://www.nuget.org/packages/CrossPlatformLiveData.Android/) provides base UI classes with LifecycleManger linked to lifecycle events:\
**LiveDataActivity, LiveDataAppCompatActivity, LiveDataAppCompatDialogFragment, LiveDataFragment, LiveDataFragmentActivity, LiveDataSupportDialogFragment, LiveDataSupportFragment**

[CrossPlatformLiveData.iOS](https://www.nuget.org/packages/CrossPlatformLiveData.iOS/) provides base UI classes with LifecycleManger linked to lifecycle events:\
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

------

[![TMS logo](https://i.imgur.com/6o5OQqZ.png)](http://tmssoft.pl/)

2020 © TMS
