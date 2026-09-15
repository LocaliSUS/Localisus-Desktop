using Localimed.Model;
using Localimed.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Localimed.ModelView;

public class RemoverMedicamentoViewModel : INotifyPropertyChanged
{
    private readonly MedicamentoApiService _api;

    public ObservableCollection<MedicamentoApi> Medicamentos { get; }
        = new();

    private MedicamentoApi? _medicamentoSelecionado;

    private string _mensagemSelecao =
        "Selecione o medicamento que deseja remover do estoque";

    public MedicamentoApi? MedicamentoSelecionado
    {
        get => _medicamentoSelecionado;
        set
        {
            if (_medicamentoSelecionado == value)
                return;

            _medicamentoSelecionado = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(PodeRemover));

            MensagemSelecao = value == null
                ? "Selecione o medicamento que deseja remover do estoque"
                : $"Medicamento selecionado: {value.NomeMedicamento}";
        }
    }

    public bool PodeRemover =>
        MedicamentoSelecionado != null;

    public string MensagemSelecao
    {
        get => _mensagemSelecao;
        set
        {
            if (_mensagemSelecao != value)
            {
                _mensagemSelecao = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand BotaoRemoverMedicamento { get; }
    public ICommand BotaoInserirMedicamentos { get; }
    public ICommand BotaoVoltar { get; }

    public RemoverMedicamentoViewModel()
    {
        _api = new MedicamentoApiService(
            new HttpClient
            {
                BaseAddress =
                    new Uri("https://localhost:7140/")
            });

        BotaoRemoverMedicamento =
            new Command(async () => await RemoverAsync());

        BotaoInserirMedicamentos =
            new Command(async () =>
                await Application.Current!.MainPage!
                    .Navigation.PushAsync(
                        new Localimed.Views.InserirMedicamentos()));

        BotaoVoltar =
            new Command(async () =>
            {
                if (Application.Current!.MainPage!
                    .Navigation.NavigationStack.Count > 1)
                {
                    await Application.Current.MainPage!
                        .Navigation.PopAsync();
                }
            });

        _ = CarregarMedicamentosAsync();
    }

    private async Task CarregarMedicamentosAsync()
    {
        var medicamentos =
            await _api.ObterMedicamentosAsync();

        Medicamentos.Clear();

        foreach (var medicamento in medicamentos)
        {
            Medicamentos.Add(medicamento);
        }
    }

    private async Task RemoverAsync()
    {
        if (MedicamentoSelecionado == null)
            return;

        var nome =
            MedicamentoSelecionado.NomeMedicamento;

        var confirmar =
            await Application.Current!.MainPage!
                .DisplayAlert(
                    "Remover medicamento",
                    $"Deseja realmente remover:\n\n{nome}?",
                    "SIM",
                    "NÃO");

        if (!confirmar)
            return;

        var sucesso =
            await _api.ExcluirMedicamentoAsync(
                MedicamentoSelecionado.IdMedicamento);

        if (!sucesso)
        {
            await Application.Current!.MainPage!
                .DisplayAlert(
                    "Erro",
                    "Não foi possível remover o medicamento.",
                    "OK");
            return;
        }

        Medicamentos.Remove(
            MedicamentoSelecionado);

        MedicamentoSelecionado = null;

        await Application.Current!.MainPage!
            .DisplayAlert(
                "Removido",
                $"O medicamento '{nome}' foi removido.",
                "OK");
    }

    public event PropertyChangedEventHandler?
        PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(name));
    }
}