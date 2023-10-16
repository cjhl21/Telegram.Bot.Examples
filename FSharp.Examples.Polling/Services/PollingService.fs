// The worker entry-point
//
// Background service consuming a scoped service.
// See more: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services#consuming-a-scoped-service-in-a-background-task
//
// This file simply forms that "link" between the .NET
// class to the actual service functions implemented in
// FSharp.Examples.Polling.Services.Internal.PollingServiceFuncs
//
// Copyright (c) 2023 Arvind Devarajan
// Licensed to you under the MIT License.
// See the LICENSE file in the project root for more information.

namespace FSharp.Examples.Polling.Services

open System
open System.Threading
open System.Threading.Tasks

open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Hosting

open FSharp.Examples.Polling.Services.Internal
open FSharp.Examples.Polling.Util

type PollingService<'T when 'T:> IReceiverService>(sp: IServiceProvider, logger: ILogger<PollingService<'T>>) =
  inherit BackgroundService()

  member __.doWork (cts: CancellationToken) = async {
    let getReceiverService _ =
      use scope = sp.CreateScope()
      let service: 'T = scope.ServiceProvider.GetRequiredService<'T>()
      service

    let cancellationNotRequested _ = (not cts.IsCancellationRequested)

    try
      Seq.initInfinite getReceiverService
      |> Seq.takeWhile cancellationNotRequested
      |> Seq.fold
    with
    | e ->
        logger.LogError($"Polling failed with exception: {e.ToString}: {e.Message}");
  }

  override __.ExecuteAsync(cts: CancellationToken) =
    logInfo logger "Starting polling service"
    __.doWork cts
