﻿using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Uno.Extensions.Reactive.Generator;

/// <summary>
/// A VM trigger parameter that can be converted into a Command.
/// </summary>
internal record CommandInput(IParameterSymbol Parameter, ITypeSymbol? _commandParameterType) : IInputInfo
{
	private readonly ITypeSymbol? _commandParameterType = _commandParameterType;

	/// <inheritdoc />
	public IParameterSymbol Parameter { get; } = Parameter;

	/// <inheritdoc />
	public string? GetBackingField()
		=> null;

	/// <inheritdoc />
	public (string? code, bool isOptional) GetCtorParameter()
		=> (null, true);

	/// <inheritdoc />
	public string? GetCtorInit(bool isInVmCtorParameters)
		=> $"var {Parameter.Name}Builder = new {NS.Reactive}.CommandBuilder<{_commandParameterType?.ToString() ?? "object?"}>(nameof({Parameter.GetPascalCaseName()}));";

	/// <inheritdoc />
	public string GetVMCtorParameter()
		=> $"{Parameter.Name}Builder as {Parameter.Type}";

	/// <inheritdoc />
	public string? GetPropertyInit()
		=> $"{Parameter.GetPascalCaseName()} = {Parameter.Name}Builder.Build({N.Ctor.Ctx});";

	/// <inheritdoc />
	public string? GetProperty()
		=> $"public {NS.Reactive}.IAsyncCommand {Parameter.GetPascalCaseName()} {{ get; }}";

	/// <inheritdoc />
	public virtual bool Equals(IInputInfo other)
		=> other is CommandInput otherCommand
			&& Parameter.Name.Equals(otherCommand.Parameter.Name, StringComparison.OrdinalIgnoreCase)
			&& SymbolEqualityComparer.Default.Equals(_commandParameterType, otherCommand._commandParameterType);
}
