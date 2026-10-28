using UnityEngine;

public class MovimentacaoTrono : MonoBehaviour
{
    
    [SerializeField] float amplitude = 0.5f; // altura do movimento
    [SerializeField] float velocidade = 2f;  // velocidade do movimento

    Vector3 posicaoInicial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        posicaoInicial = transform.position;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * velocidade) * amplitude;
        transform.position = posicaoInicial + new Vector3(0, y, 0);
    }
}
