# BackgroundWorker Component and .NET 6

## Purpose  
So, it turns out that the BackgroundWorker component is still around in .NET 6. Although, I generally work with Tasks for asynchronous code, I have a soft spot for the BackgroundWorker because it is one of the first topics I spoke on publicly: [Keep Your UI Responsive with the BackgroundWorker Component](http://www.jeremybytes.com/Demos.aspx#KYUIR). (It's also a really easy way to get something off of the UI thread of a desktop application.)  

I'm actually surprised that it has not been deprecated yet. So, just because I could, I created .NET 6 version of my BackgroundWorker sample code.

## Projects  
The solution consists of 3 projects:  
* **SlowLibrary**  
This is the long-running process that we want to run on a background thread.
* **BGW.WPF**  
A WPF desktop application that uses the BackgroundWorker to run the SlowLibrary code in the background (with progress reporting and cancellation).  
* **BGW.MVVM**  
The same functionality as the WPF application, but uses the MVVM design pattern. The BackgroundWorker code is in the View Model of the application.  
* **Task.MVVM**  
The same functionality as the BGW.MVVM application, but this shows how to use Task and IProgress&lt;T&gt; instead of the BackgroundWorker Component.

## BackgroundWorker
What I like about the BackgroundWorker component is that it has just a few events, methods, and properties to get the functionality of running something on a background thread, reporting progress, handling errors, and dealing with cancellation.  

**Background Processing**
* DoWork (event)
* RunWorkerCompleted (event)
* RunWorkerAsync (method)
* IsBusy (property)

**Progress Reporting**
* WorkerReportsProgress (property)
* ReportProgress (method)
* ProgressChanged (event)

**Cancellation**
* WorkerSupportsCancellation (property)
* CancellationPending (property)

## Resources
If you want some details on the various BackgroundWorker members and how they work, here are a few things you can look at.
* Article: [Introduction to the BackgroundWorker Component in WPF](https://jeremybytes.blogspot.com/2009/12/introduction-to-background-worker.html) (2009)
* Session Materials: [Keep Your UI Responsive with the BackgroundWorker Component](http://www.jeremybytes.com/Demos.aspx#KYUIR) (2011)
* Pluralsight Course: [Introduction to the .NET BackgroundWorker Component](http://www.pluralsight.com/training/Courses/TableOfContents/dotnet-backgroundworker-component-introduction) (2013) 

These resources are all pretty old, but the basics are all the same. If anyone is interested, I'll be glad to do an updated article that is specific to these samples. But this .NET 6 update was mainly done out of curiosity.
