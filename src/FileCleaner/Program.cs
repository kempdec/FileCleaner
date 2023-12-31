﻿using KempDec.FileCleaner.Core;
using KempDec.FileCleaner.Models;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using static System.Console;
using static KempDec.FileCleaner.Helpers.ConsoleHelper;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .Build();

AppSettings? appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

if (appSettings is null)
{
    WriteErrorLine($"A seção '{nameof(AppSettings)}' no arquivo de configuração appsettings.json não foi encontrado.");

    ReadLine();

    return;
}

if (appSettings.CleaningTasks is not { Count: > 0 })
{
    WriteErrorLine("Nenhuma tarefa de limpeza foi encontrada.");

    ReadLine();

    return;
}

foreach (CleaningTask cleaningTask in appSettings.CleaningTasks)
{
    if (string.IsNullOrEmpty(cleaningTask.Path))
    {
        WriteErrorLine("O caminho da tarefa de limpeza está vazio.");

        continue;
    }

    CleanFiles(cleaningTask.Path, cleaningTask.FilesDaysAgo);
}

while (true)
{
    ReadLine();
}

static void CleanFiles(string path, int? daysAgo)
{
    try
    {
        var cleaner = new FileCleaner(path);

        WriteLine("==========================================================");
        WriteLine($"Caminho: {cleaner.Path}");
        WriteLine("Iniciando a limpeza...");

        ForegroundColor = ConsoleColor.Green;

        var progress = new Progress<(int Count, int TotalCount)>(WriteProgress);
        var stopwatch = Stopwatch.StartNew();

        int count = daysAgo is null
            ? cleaner.DeleteAllFiles(progress)
            : cleaner.DeleteAllOldFiles(daysAgo.Value, progress);

        stopwatch.Stop();

        ResetColor();

        if (count > 0)
        {
            // Escreve uma linha para evitar que o resultado seja escrito na frente do progresso.
            WriteLine();
        }

        WriteLine($"Limpeza concluída. {count} arquivos foram excluídos em {stopwatch.Elapsed.TotalSeconds:N2}s!");
    }
    catch (Exception ex)
    {
        WriteErrorLine(ex.Message);
    }
}

static void WriteProgress((int Count, int TotalCount) progress)
{
    WriteReplace($"Por favor, aguarde. Excluindo {progress.Count}/{progress.TotalCount} arquivos.");
}
