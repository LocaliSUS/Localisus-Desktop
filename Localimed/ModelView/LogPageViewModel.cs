using System.ComponentModel;
using System.Windows.Input;

namespace Localimed.ViewModel;

public class LogPageViewModel : INotifyPropertyChanged
//BindableObject, se trata de um mecanismo de armazenamento de 
//dados que durante o desenvolvimento da aplicação exista
//uma sincronia entre a View e a ViewModel!
{
    private bool _botaoAtivo;
    private string _identificacaoUsuario;

    //criando aqui os objetos para o bindingcontext da views

    //criando um objeto público que vai permitir a conexão entre
    //as telas, essa classe vai receber os atributos da privada
    //Com essa classe iremos implementar o começo da lógica
    public string IdentificacaoUsuario
    {
        get => _identificacaoUsuario;
        set
        {
            //aqui estamos passando a identificação do usuário
            //como um valor, caso ela esteja vazia o nosso botão não 
            //estará ativo!
            if (_identificacaoUsuario != value)
            {
                _identificacaoUsuario = value;
                OnPropertyChanged(nameof(IdentificacaoUsuario));
            }


        }
    }

    //criando agora um objeto público para navegação entre telas
    //com a lógica do botão verificando se está ativo ou não!
    //public bool BotaoAtivo
    //{
    //    get => _botaoAtivo;
    //    set
    //    {
    //        _botaoAtivo = value;
    //        OnPropertyChanged();
    //    }
    //}

    //public ICommand será o comando que substituíra o Clicked q estava presente em LogPage.xaml.cs
    //Agora esse command deverá ser instânciado na nossa View sendo
    //a nossa primeira BindingContext
    public ICommand BotaoContinuar { get; }

    public LogPageViewModel()
    {

        BotaoContinuar = new Command(OnContinuarClicked);

    }

    public async void OnContinuarClicked()
    {

        /*
        ============================================================= 
            LÓGICA PARA O BOTÃO CONTINUAR EMBAIXO DO ID FUNCIONÁRIO
        =======================================================
        */
        if (string.IsNullOrWhiteSpace(IdentificacaoUsuario))
        {
            await Application.Current.MainPage.DisplayAlert(
                "Erro",
                "Verifique se o campo de Identificação está vazio!",
                "Ok"
                );
            return;
        }

        if (IdentificacaoUsuario == "1")
        {

            await Application.Current.MainPage.DisplayAlert("Sucesso", "Conexão certinha", "jaé neguin");

            await Application.Current.MainPage.Navigation.PushAsync(new Views.HomePage());

        }
        else
        {
            await Application.Current.MainPage.DisplayAlert(
                "Erro",
                "Usuário não encontrado! Confirme o ID",
                "Ok"
                );
            return;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


}