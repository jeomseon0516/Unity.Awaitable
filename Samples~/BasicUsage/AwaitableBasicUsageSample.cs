using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jeomseon.Unity.Awaitable.Samples.BasicUsage
{
    /// <summary>
    /// 세 비동기 작업의 조합과 조건 대기를 오브젝트 색 변화로 보여주는 실행 예제입니다.
    /// </summary>
    public sealed class AwaitableBasicUsageSample : MonoBehaviour
    {
        private static readonly Color PendingColor = new(0.18f, 0.22f, 0.28f);
        private static readonly Color CompletedColor = new(0.15f, 0.8f, 0.45f);
        private static readonly Color WaitingColor = new(1f, 0.65f, 0.1f);
        private static readonly Color AllCompletedColor = new(0.1f, 0.65f, 1f);

        [SerializeField, Min(0.1f)] private float firstDelay = 0.5f;
        [SerializeField, Min(0.1f)] private float secondDelay = 1f;
        [SerializeField, Min(0.1f)] private float thirdDelay = 1.5f;

        private readonly List<Material> _materials = new();
        private int _completedCount;

        private async void Start()
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
            {
                Debug.LogError("Awaitable Basic Usage requires the Unity 6000.6 URP environment.", this);
                return;
            }

            var first = CreateIndicator("First", new Vector3(-2f, 0f, 0f), PendingColor, shader);
            var second = CreateIndicator("Second", Vector3.zero, PendingColor, shader);
            var third = CreateIndicator("Third", new Vector3(2f, 0f, 0f), PendingColor, shader);
            var status = CreateIndicator("Combined Status", new Vector3(0f, -2f, 0f), WaitingColor, shader);

            try
            {
                var token = destroyCancellationToken;
                var untilAllComplete = AwaitableUtility.WaitUntil(() => _completedCount == 3, token);
                var whileIncomplete = AwaitableUtility.WaitWhile(() => _completedCount < 3, token);

                await AwaitableUtility.WhenAll(
                    CompleteAfter(first, firstDelay, token),
                    CompleteAfter(second, secondDelay, token),
                    CompleteAfter(third, thirdDelay, token));
                await untilAllComplete;
                await whileIncomplete;

                SetColor(status, AllCompletedColor);
                Debug.Log("Awaitable Basic Usage completed: WhenAll, WaitUntil, and WaitWhile succeeded.", this);
            }
            catch (OperationCanceledException)
            {
                // Scene 종료로 인한 정상 취소입니다.
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }
        }

        private async UnityEngine.Awaitable CompleteAfter(
            Renderer indicator,
            float delay,
            System.Threading.CancellationToken cancellationToken)
        {
            await UnityEngine.Awaitable.WaitForSecondsAsync(delay, cancellationToken);
            SetColor(indicator, CompletedColor);
            _completedCount++;
        }

        private Renderer CreateIndicator(string objectName, Vector3 position, Color color, Shader shader)
        {
            var indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
            indicator.name = objectName;
            indicator.transform.SetParent(transform, false);
            indicator.transform.localPosition = position;

            var material = new Material(shader) { name = $"{objectName} (Runtime)" };
            _materials.Add(material);

            var renderer = indicator.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            SetColor(renderer, color);
            return renderer;
        }

        private static void SetColor(Renderer renderer, Color color)
        {
            renderer.sharedMaterial.SetColor("_BaseColor", color);
        }

        private void OnDestroy()
        {
            foreach (var material in _materials)
            {
                if (material != null) Destroy(material);
            }
        }
    }
}
