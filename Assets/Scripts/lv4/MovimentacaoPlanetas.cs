using UnityEngine;

public class MovimentacaoPlanetas : MonoBehaviour
{
    [SerializeField] float velocidade = 5f;
    [SerializeField] float velocidadeRotacao = 90f;

    void Update()
    {
        // Space.World garante que ele sempre vai para frente no mundo, ignorando a rotação
        transform.Translate(Vector3.right * velocidade * Time.deltaTime, Space.World);

        // Rotaciona no próprio eixo Y
        transform.Rotate(Vector3.up * velocidadeRotacao * Time.deltaTime);
    }
}