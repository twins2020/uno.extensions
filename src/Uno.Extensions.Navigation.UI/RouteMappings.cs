﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Uno.Extensions.Navigation;

public class RouteMappings : IMappings
{
	protected IDictionary<string, RouteMap> Mappings { get; } = new Dictionary<string, RouteMap>();
	protected IDictionary<string, ViewMap> ViewMappings { get; } = new Dictionary<string, ViewMap>();

	protected ILogger Logger { get; }

	protected RouteMappings(ILogger logger, IEnumerable<RouteMap> maps, IEnumerable<ViewMap> viewMaps)
	{
		Logger = logger;
		if (maps is not null)
		{
			maps.ForEach(map => Mappings[map.Path] = map);
		}
		if (viewMaps is not null)
		{
			viewMaps.ForEach(map=> ViewMappings[map.Path] = map);
		}
	}

	public RouteMappings(ILogger<RouteMappings> logger, IEnumerable<RouteMap> maps, IEnumerable<ViewMap> viewMaps) : this((ILogger)logger, maps, viewMaps)
	{
	}

	public RouteMap? Find(Route route) => FindByPath(route.Base);

	public virtual RouteMap? FindByPath(string? path)
	{
		if (
			path is null ||
			string.IsNullOrWhiteSpace(path) ||
			path == Schemes.Parent ||
			path == Schemes.Current)
		{
			return null;
		}

		return Mappings.TryGetValue(path, out var map) ? map : default;
	}

	public RouteMap? FindByViewModel(Type? viewModelType)
	{
		return FindRouteByViewMapType(viewModelType, map => map.ViewModelType);
	}

	public RouteMap? FindByView(Type? viewType)
	{
		return FindRouteByViewMapType(viewType, map => map.ViewType);
	}

	public virtual RouteMap? FindByData(Type? dataType)
	{
		return FindRouteByType(dataType, map => map.Data);
	}

	public virtual RouteMap? FindByResultData(Type? dataType)
	{
		return FindRouteByType(dataType, map => map.ResultData);
	}

	private RouteMap? FindRouteByViewMapType(Type? typeToFind, Func<ViewMap, Type?> mapType)
	{
		var viewMap = FindByInheritedTypes(ViewMappings, typeToFind, mapType);
		return FindByPath(viewMap?.Path);
	}

	private RouteMap? FindRouteByType(Type? typeToFind, Func<RouteMap, Type?> mapType)
	{
		return FindByInheritedTypes(Mappings, typeToFind, mapType);
	}


	private ViewMap? FindViewByRouteMapType(Type? typeToFind, Func<RouteMap, Type?> mapType)
	{
		var routeMap = FindByInheritedTypes(Mappings, typeToFind, mapType);
		return FindViewByPath(routeMap?.Path);
	}

	private ViewMap? FindViewByType(Type? typeToFind, Func<ViewMap, Type?> mapType)
	{
		return FindByInheritedTypes(ViewMappings, typeToFind, mapType);
	}

	private TMap? FindByInheritedTypes<TMap>(IDictionary<string, TMap> mappings, Type? typeToFind, Func<TMap, Type?> mapType)
	{
		// Handle the non-reflection check first
		var map = (from m in mappings
				   where mapType(m.Value) == typeToFind
				   select m.Value)
				   .FirstOrDefault();
		if (map is not null)
		{
			return map;
		}

		return (from baseType in typeToFind.GetBaseTypes()
				from m in mappings
				where mapType(m.Value) == baseType
				select m.Value)
				   .FirstOrDefault();
	}

	public ViewMap? FindView(Route route) => FindViewByPath(route.Base);
	public virtual ViewMap? FindViewByPath(string? path)
	{
		if (
			path is null ||
			string.IsNullOrWhiteSpace(path) ||
			path == Schemes.Parent ||
			path == Schemes.Current)
		{
			return null;
		}

		return ViewMappings.TryGetValue(path, out var map) ? map : default;
	}

	public virtual ViewMap? FindViewByViewModel(Type? viewModelType)
	{
		return FindViewByType(viewModelType, map => map.ViewModelType);
	}
	public virtual ViewMap? FindViewByView(Type? viewType)
	{
		return FindViewByType(viewType, map => map.ViewType);
	}

	public ViewMap? FindViewByData(Type? dataType)
	{
		return FindViewByRouteMapType(dataType, map => map.Data);
	}

	public ViewMap? FindViewByResultData(Type? resultDataType)
	{
		return FindViewByRouteMapType(resultDataType, map => map.ResultData);
	}
}
