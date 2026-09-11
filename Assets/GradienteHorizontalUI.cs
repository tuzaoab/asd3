using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Horizontal Gradient")]
public class GradienteHorizontalUI : BaseMeshEffect
{
    // Cor do Topo
    public Color corEsquerda = new Color(1, 1, 1, 0f);

    // Cor da Base
    public Color corDireita = new Color(1, 1, 1, 0.5f);

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive() || vh.currentVertCount == 0) return;

        UIVertex v = new UIVertex();

        // Encontra a menor e maior posição Y na malha do objeto
        float yMin = float.MaxValue;
        float yMax = float.MinValue;

        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref v, i);
            if (v.position.y < yMin) yMin = v.position.y;
            if (v.position.y > yMax) yMax = v.position.y;
        }

        float altura = yMax - yMin;
        if (Mathf.Approximately(altura, 0f)) return;

        // Aplica o gradiente com o fade invertido
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref v, i);

            // 't' varia de 0 (base) a 1 (topo)
            float t = (v.position.y - yMin) / altura;

            // Invertemos a ordem (corEsquerda <-> corDireita) para inverter o fade
            v.color = Color.Lerp(corEsquerda, corDireita, t);

            vh.SetUIVertex(v, i);
        }
    }
}