mergeInto(LibraryManager.library, {
    EnviarDesempenho: function(jsonPtr) {
        var json = UTF8ToString(jsonPtr);
        var dados = JSON.parse(json);
        var token = localStorage.getItem('token');

        if (!token) {
            console.warn('Token JWT nao encontrado no localStorage.');
            return;
        }

        // URL relativa: funciona em localhost, em IP de rede local e sob o tunel,
        // sem precisar rebuildar o jogo quando o endereco mudar.
        fetch('/api/Desempenho', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            },
            body: JSON.stringify(dados)
        })
        .then(function(res) {
            if (res.ok) {
                console.log('Desempenho enviado com sucesso!');
            } else {
                console.error('Erro ao enviar desempenho:', res.status);
            }
        })
        .catch(function(err) {
            console.error('Erro de rede:', err);
        });
    },

    IniciarListenerMensagens: function() {
        window.addEventListener('message', function(event) {
            if (event.data === 'enviarDadosParciais') {
                if (typeof SendMessage === 'function') {
                    SendMessage('ApiManager', 'EnviarDadosParciais');
                }
            }
        });
    },

    SalvarRespostasHistoria: function(jsonPtr) {
        var json = UTF8ToString(jsonPtr);
        localStorage.setItem('historiaRespostas', json);
    },

    CarregarRespostasHistoria: function() {
        var dados = localStorage.getItem('historiaRespostas') || '{"items":[]}';
        var bufferSize = lengthBytesUTF8(dados) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(dados, buffer, bufferSize);
        return buffer;
    },

    LimparRespostasHistoria: function() {
        localStorage.removeItem('historiaRespostas');
    }
});
