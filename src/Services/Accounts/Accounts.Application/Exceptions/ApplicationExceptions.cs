namespace Accounts.Application.Exceptions;

/// <summary>Excepción de recurso inexistente.</summary>
public sealed class NotFoundException(string message) : Exception(message);

/// <summary>Excepción de conflicto con el estado actual.</summary>
public sealed class ConflictException(string message) : Exception(message);

/// <summary>Excepción de entrada inválida o regla de aplicación incumplida.</summary>
public sealed class ValidationException(string message) : Exception(message);