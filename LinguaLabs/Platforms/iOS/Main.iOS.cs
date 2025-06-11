using LinguaLabs;
using UIKit;
using Uno.UI.Hosting;

var host = UnoPlatformHostBuilder.Create()
    .App(() => new LinguaLabs.App())
    .UseAppleUIKit()
    .Build();

host.Run();
