﻿using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record ActionDetails(JObject Wrapped, InvokeOptions Options) : IHasExtensions, IHasLinks;