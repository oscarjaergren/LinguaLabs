global using System.Collections.Immutable;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Localization;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using LinguaLabs.Models;
global using LinguaLabs.Presentation;
global using LinguaLabs.Services.Endpoints;
global using LinguaLabs.DataContracts;
global using LinguaLabs.DataContracts.Serialization;
global using LinguaLabs.Services.Caching;
global using LinguaLabs.Client;
global using Uno.Extensions.Http.Kiota;
#if MAUI_EMBEDDING
global using LinguaLabs.MauiControls;
#endif
global using ApplicationExecutionState = Windows.ApplicationModel.Activation.ApplicationExecutionState;
[assembly: Uno.Extensions.Reactive.Config.BindableGenerationTool(3)]
