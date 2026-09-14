using Actors;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Game
{
    public sealed class UiFactory : System.IDisposable
    {
        private readonly Sprite white = Sprite.Create(Texture2D.whiteTexture,
            new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height), new Vector2(0.5f, 0.5f));
        public void Dispose() => Object.Destroy(white);
        private readonly Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        private static readonly Color Panel = new Color(0.04f, 0.07f, 0.11f, 0.94f);
        private static readonly Color TextColor = new Color(0.9f, 0.94f, 1);

        public void CreateWorldBar(Combatant actor, Camera camera)
        {
            var root = new GameObject("World health", typeof(RectTransform), typeof(Canvas));
            root.transform.SetParent(actor.transform, false);
            root.transform.localPosition = Vector3.up * 2.2f;
            root.transform.localScale = Vector3.one * 0.006f;
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = camera;
            canvas.sortingOrder = 1;
            var rect = (RectTransform)root.transform;
            rect.sizeDelta = new Vector2(180, 18);
            var background = Image("Background", rect, new Vector2(180, 18), Vector2.zero, Panel);
            var fill = Image("Health", background.rectTransform, new Vector2(172, 10), Vector2.zero, Color.green);
            Fill(fill);
            root.AddComponent<HealthBar>().Initialize(actor.Health, fill, null, camera);
        }
        
        public void CreateHud(Combatant player, ActorCombat combat)
        {
            var root = new GameObject("HUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.matchWidthOrHeight = 0.5f;
            var canvas = (RectTransform)root.transform;
            var stats = Image("Stats", canvas, new Vector2(280, 138), new Vector2(24, -24), Panel);
            stats.rectTransform.anchorMin = stats.rectTransform.anchorMax = new Vector2(0, 1);
            stats.rectTransform.pivot = new Vector2(0, 1);
            Label("RPG", stats.rectTransform, new Vector2(240, 26), new Vector2(0, 45), 16);
            var hpBack = Image("Health background", stats.rectTransform, new Vector2(240, 23), new Vector2(0, 12), Color.black);
            var hp = Image("Health fill", hpBack.rectTransform, new Vector2(236, 19), Vector2.zero, Color.green);
            Fill(hp);
            var hpText = Label("HP", hpBack.rectTransform, new Vector2(230, 23), Vector2.zero, 15);
            root.AddComponent<HealthBar>().Initialize(player.Health, hp, hpText);
            var magic = Image("Magic cooldown", stats.rectTransform, new Vector2(240, 25), new Vector2(0, -27), new Color(0.15f, 0.45f, 0.8f));
            Fill(magic);
            var magicText = Label("MAGIC", stats.rectTransform, new Vector2(240, 25), new Vector2(0, -27), 15);
            Label("+", canvas, new Vector2(30, 30), Vector2.zero, 22);
            var help = Label("WASD / ARROWS  Move     SHIFT  Run     LMB  Sword     RMB  Magic     ESC  Cursor", canvas,
                new Vector2(1180, 36), new Vector2(0, 22), 17);
            help.rectTransform.anchorMin = help.rectTransform.anchorMax = new Vector2(0.5f, 0);

            var overlay = Image("Game over", canvas, new Vector2(1280, 720), Vector2.zero, new Color(0, 0, 0, 0.7f));
            overlay.rectTransform.anchorMin = Vector2.zero;
            overlay.rectTransform.anchorMax = Vector2.one;
            overlay.rectTransform.sizeDelta = Vector2.zero;
            overlay.raycastTarget = true;
            Label("GAME OVER", overlay.rectTransform, new Vector2(600, 80), new Vector2(0, 60), 48);
            var buttonImage = Image("Restart", overlay.rectTransform, new Vector2(240, 60), new Vector2(0, -35), new Color(0.1f, 0.4f, 0.65f));
            buttonImage.raycastTarget = true;
            var button = buttonImage.gameObject.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            Label("RESTART", buttonImage.rectTransform, new Vector2(240, 60), Vector2.zero, 22);
            var hud = root.AddComponent<GameHud>();
            hud.Initialize(player.Health, combat, magic, magicText, overlay.gameObject);
            button.onClick.AddListener(hud.Restart);
            var events = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
        }
        private static void Fill(Image image)
        {
            image.type = UnityEngine.UI.Image.Type.Filled;
            image.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
            image.fillOrigin = 0;
        }
        private Image Image(string name, RectTransform parent, Vector2 size, Vector2 position, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            var rect = (RectTransform)go.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size; rect.anchoredPosition = position;
            var image = go.GetComponent<Image>();
            image.sprite = white; image.color = color; image.raycastTarget = false;
            return image;
        }
        private Text Label(string text, RectTransform parent, Vector2 size, Vector2 position, int fontSize)
        {
            var go = new GameObject(text, typeof(RectTransform), typeof(Text));
            var rect = (RectTransform)go.transform;
            rect.SetParent(parent, false); rect.sizeDelta = size; rect.anchoredPosition = position;
            var label = go.GetComponent<Text>();
            label.font = font; label.fontSize = fontSize; label.color = TextColor; label.text = text;
            label.alignment = TextAnchor.MiddleCenter; label.raycastTarget = false;
            return label;
        }
    }
}
