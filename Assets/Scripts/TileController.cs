using UnityEngine;

public class TileController : MonoBehaviour {

    /// <summary>
    /// Link to background sprite
    /// </summary>
    [SerializeField]
    private SpriteRenderer Background = null;

    /// <summary>
    /// Color cache
    /// </summary>
    private Color _color = Color.white;

    /// <summary>
    /// Init field
    /// </summary>
    public void Init () {
        float alpha = Random.Range (Config.MIN_LIGHT_POWER, Config.MAX_LIGHT_POWER);
        _color.a = alpha;
        Background.color = _color;
    }
}