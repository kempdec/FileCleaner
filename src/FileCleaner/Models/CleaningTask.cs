﻿namespace KempDec.FileCleaner.Models;

/// <summary>
/// Representa uma tarefa de limpeza das configurações do aplicativo.
/// </summary>
internal record CleaningTask
{
    /// <summary>
    /// Obtém ou inicializa o caminho da tarefa de limpeza.
    /// </summary>
    public string? Path { get; init; }

    /// <summary>
    /// Obtém ou inicializa a quantidade de dias atrás para um arquivo ser excluído.
    /// </summary>
    public int? FilesDaysAgo { get; init; }
}
