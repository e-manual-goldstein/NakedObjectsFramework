﻿using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ServicesApi {
    public static IEnumerable<Link> GetValue(this Services servicesRepresentation) => servicesRepresentation.GetMandatoryProperty(JsonConstants.Value).ToLinks(servicesRepresentation);

    public static Link? GetServiceLink(this Services servicesRepresentation, string serviceId) =>
        servicesRepresentation.GetValue().SingleOrDefault(l => l.GetRel().GetServiceId() == serviceId);

    public static Link? GetServiceLinkByTitle(this Services servicesRepresentation, string serviceTitle) =>
        servicesRepresentation.GetValue().SingleOrDefault(l => l.GetTitle() == serviceTitle);

    public static async Task<DomainObject?> GetService(this Services servicesRepresentation, string serviceId, InvokeOptions? options = null) =>
        servicesRepresentation.GetServiceLink(serviceId) is { } l ? new DomainObject(await ApiHelpers.GetResourceAsync(l, options ?? servicesRepresentation.Options), servicesRepresentation.Options) : null;

    public static async Task<DomainObject?> GetServiceByTitle(this Services servicesRepresentation, string serviceTitle, InvokeOptions? options = null) =>
        servicesRepresentation.GetServiceLinkByTitle(serviceTitle) is { } l ? new DomainObject(await ApiHelpers.GetResourceAsync(l, options ?? servicesRepresentation.Options), servicesRepresentation.Options) : null;
}