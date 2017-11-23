using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Script
{
    public class MagicWordScript : MonoBehaviour
    {
        public TextMeshPro RandomLetterProto;
        public TextMeshPro MagicWordProto;

        List<TextMeshPro> letterObjects_ = new List<TextMeshPro>();
        string letters_;

        void Awake()
        {
            letters_ = MagicWordProto.text;
            CreateLetterObjects();
        }

        void Start()
        {
            StartCoroutine(ColorCycleCo());
            StartCoroutine(SpawnLetterCo());
        }

        IEnumerator SpawnLetterCo()
        {
            float nextRnd = 0.0f;
            Transform trans = RandomLetterProto.GetComponent<Transform>();
            Vector3 basePos = trans.localPosition;

            while (true)
            {
                float t = Time.time;
                if (t > nextRnd)
                {
                    int rnd = Random.Range(0, letters_.Length);
                    RandomLetterProto.text = letters_[rnd].ToString();
                    nextRnd = t + 0.05f;
                }

                float sinSpeed = 5.0f;
                float sinAmount = Mathf.Sin(t) * 3;
                float sinScale = 100.0f;
                float sinX = Mathf.Sin(t * sinSpeed);
                float sinY = Mathf.Sin((t + 3.1415f * 0.5f) * sinSpeed);
                Vector3 pos = basePos;
                pos.x += sinX * sinAmount * sinScale;
                pos.y += sinY * sinAmount * sinScale;
                trans.localPosition = pos;

                yield return null;
            }
        }

        float h = 0;
        float s = 1.0f;
        float v = 0.3f;

        IEnumerator ColorCycleCo()
        {
            while (true)
            {
//                yield return new WaitForSeconds(5.0f);

                float stepH = 0.1f;
                for (int i = 0; i < letterObjects_.Count; ++i)
                {
                    float localH = h + stepH * i * 0.2f;
                    if (localH > 1.0f)
                        localH -= 1.0f;

                    Color flashColor = Color.HSVToRGB(localH, s, v);

                    var letter = letterObjects_[i];
                    letter.color = flashColor;
//                    yield return new WaitForSeconds(0.2f);
                }

                h += Time.deltaTime * 0.02f;
                if (h > 1.0f)
                    h -= 1.0f;
                yield return null;
            }
        }

        void CreateLetterObjects()
        {
            MagicWordProto.ForceMeshUpdate();

            Transform protoTransform = MagicWordProto.GetComponent<Transform>();
            var textInfo = MagicWordProto.textInfo;

            letterObjects_.Clear();
            for (int i = 0; i < letters_.Length; ++i)
            {
                string letter = letters_[i].ToString();
                var tlp = GameObject.Instantiate<TextMeshPro>(MagicWordProto);
                tlp.text = letters_[i].ToString();

                var charInfo = textInfo.characterInfo[i];
                Vector3 bl = protoTransform.TransformPoint(charInfo.bottomLeft);
                Vector3 tl = protoTransform.TransformPoint(new Vector3(charInfo.topLeft.x, charInfo.topLeft.y, 0));
                Vector3 tr = protoTransform.TransformPoint(charInfo.topRight);
                Vector3 br = protoTransform.TransformPoint(new Vector3(charInfo.bottomRight.x, charInfo.bottomRight.y, 0));
                Vector3 center = Vector3.Lerp(tl, br, 0.5f);
                tlp.transform.position = center;
                tlp.transform.SetParent(transform, true);

                letterObjects_.Add(tlp);
            }

            MagicWordProto.gameObject.SetActive(false);
        }

        void Update()
        {

        }
    }
}