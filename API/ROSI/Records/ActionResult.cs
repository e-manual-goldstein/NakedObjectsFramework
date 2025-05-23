﻿using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record ActionResult(JObject Wrapped, InvokeOptions Options, EntityTagHeaderValue? Tag) : IHasExtensions, IHasLinks;