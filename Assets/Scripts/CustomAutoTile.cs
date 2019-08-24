using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps
{
	[Serializable]
	public class CustomAutoTile : TileBase
	{
		[SerializeField]
		public Sprite[] m_RawTilesSprites;


		public Sprite[] m_PatternedSprites;

		public override void RefreshTile(Vector3Int location, ITilemap tileMap)
        {
            if (m_PatternedSprites != null)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        base.RefreshTile(location + new Vector3Int(x, y, 0), tileMap);
                    }
                }
            }
            else
            {
                base.RefreshTile(location, tileMap);
            }
        }

		public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
		{
			UpdateTile(location, tileMap, ref tileData);
			return;
		}

		private void UpdateTile(Vector3Int location, ITilemap tileMap, ref TileData tileData)
		{
			if (m_PatternedSprites == null)
			{
				if (m_RawTilesSprites[0] && m_RawTilesSprites[1] && m_RawTilesSprites[2] && m_RawTilesSprites[3] && m_RawTilesSprites[4])
				{
					GeneratePatterns();
				}
				else
				{
					return;
				}
			}
			tileData.transform = Matrix4x4.identity;
			tileData.color = Color.white;

			int mask = TileValue(tileMap, location + new Vector3Int(0, 1, 0)) ? 1 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(1, 1, 0)) ? 2 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(1, 0, 0)) ? 4 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(1, -1, 0)) ? 8 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(0, -1, 0)) ? 16 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(-1, -1, 0)) ? 32 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(-1, 0, 0)) ? 64 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(-1, 1, 0)) ? 128 : 0;

			int index = GetIndex((byte)mask);
			if (index >= 0 && index < m_PatternedSprites.Length && TileValue(tileMap, location))
			{
				tileData.sprite = m_PatternedSprites[index];
				tileData.color = Color.white;
				tileData.flags = (TileFlags.LockTransform | TileFlags.LockColor);
				tileData.colliderType = Tile.ColliderType.Sprite;
			}

            // 周囲に同じタイルがない場合のスプライトはインポートされたものを直接使用する。
            // タイルのプレビューを TilePalette に表示するため (コードで生成したスプライトは TilePalette に表示されない)。
            if (index == m_PatternedSprites.Length-1)
            {
                tileData.sprite = m_RawTilesSprites[13];
            } 
		}

		private bool TileValue(ITilemap tileMap, Vector3Int position)
		{
			TileBase tile = tileMap.GetTile(position);
			return (tile != null && tile == this);
		}

		private int GetIndex(byte mask)
		{
			string[] patternTexts = {
				"x0x111x0",
				"x11111x0",
				"x111x0x0",
				"x10111x0",
				"x11101x0",
				"01111111",
				"11111101",
				"x0x1x0x0",
				"x0x11111",
				"11111111",
				"1111x0x1",
				"x0x10111",
				"1101x0x1",
				"11011111",
				"11110111",
				"x0x1x0x1",
				"x0x0x111",
				"11x0x111",
				"11x0x0x1",
				"x0x11101",
				"0111x0x1",
				"01110111",
				"11011101",
				"x0x0x0x1",
				"x0x101x0",
				"x10101x0",
				"x101x0x0",
				"01x0x111",
				"11x0x101",
				"11010101",
				"01010111",
				"11010111",
				"x0x10101",
				"01010101",
				"0101x0x1",
				"11110101",
				"01011111",
				"01110101",
				"01011101",
				"01111101",
				"x0x0x101",
				"01x0x101",
				"01x0x0x1",
				"x0x0x1x0",
				"x1x0x1x0",
				"x1x0x0x0",
				"x0x0x0x0"
			};
			int index = -1;
			for (int j = 0; j < patternTexts.Length; j++)
			{
				bool flag = true;
				for (int i = 0; i < 8; i++)
				{
					if (patternTexts[j][i] != 'x')
					{
						char currentBitChar = ((mask & (byte)Mathf.Pow(2, 7 - i)) != 0) ? '1' : '0';
						if (patternTexts[j][i] != currentBitChar)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					index = j;
					break;
				}
			}
			return index;

		}



		Sprite[,] Segments = new Sprite[14, 4];
		int[][] Patterns = new int[][]
		{
			//new int[] {0,2,1,4},
			new int[] {0,0,0,0},	//
			new int[] {1,1,1,1},	//
			new int[] {2,2,2,2},	//
			new int[] {1,1,11,0},
			new int[] {1,1,12,12},	//
			new int[] {9,9,9,9},	//
			new int[] {10,10,10,10},	//
			new int[] {0,2,3,5},	//tate
			new int[] {3,3,3,3},	//
			new int[] {4,4,4,4},	// 空洞
			new int[] {5,5,5,5},	//
			new int[] {3,6,3,12},	
			new int[] {8,5,11,5},
			new int[] {11,11,11,11},	//
			new int[] {12,12,12,12},	//
			new int[] {3,5,3,5},	//

			new int[] {6,6,6,6},	//
			new int[] {7,7,7,7},	//
			new int[] {8,8,8,8},	//
			new int[] {3,10,3,0},
			new int[] {9,5,2,5},
			new int[] {9,6,2,12},
			new int[] {8,10,11,0},
			new int[] {6,8,6,8},	//

			new int[] {0,0,0,12},
			new int[] {1,1,11,12},
			new int[] {2,2,11,2},
			new int[] {9,6,7,7},
			new int[] {8,10,7,7},
			new int[] {8,10,11,12},
			new int[] {9,6,11,12},
			new int[] {11,12,11,12},	//

			new int[] {3,10,3,12},	//ト

			new int[] {9,10,11,12}, 	//十字

			new int[] {9,5,11,5},	//
			new int[] {10,10,12,12},	//
			new int[] {9,9,11,11},	//
			new int[] {9,10,2,12},
			new int[] {9,10,11,11},
			new int[] {9,10,9,10},	//

			new int[] {1,3,0,2},
			new int[] {9,10,7,7},
			new int[] {9,8,8,8},	//角右上
			new int[] {0,0,6,6},	//左
			new int[] {1,1,7,7},	//中
			new int[] {2,2,8,8},	//右
			new int[] {13,13,13,13}		//孤立

		};
		public void GeneratePatterns()
		{
			for (int i = 0; i < 14; i++)
			{
				Texture2D tex = m_RawTilesSprites[i].texture;
				int y = (int)m_RawTilesSprites[i].rect.y;
				int x = (int)m_RawTilesSprites[i].rect.x;
				int height = (int)m_RawTilesSprites[i].rect.height;
				int width = (int)m_RawTilesSprites[i].rect.width;
				int height_half = height / 2;
				int width_half = width / 2;
				Segments[i, 0] = Sprite.Create(tex, new Rect(x, y, width_half, height_half), Vector2.zero);
				Segments[i, 1] = Sprite.Create(tex, new Rect(x + width_half, y, width_half, height_half), Vector2.zero);
				Segments[i, 2] = Sprite.Create(tex, new Rect(x, y + height_half, width_half, height_half), Vector2.zero);
				Segments[i, 3] = Sprite.Create(tex, new Rect(x + width_half, y + height_half, width_half, height_half), Vector2.zero);

			}

			m_PatternedSprites = new Sprite[47];
			for (int i = 0; i < 47; i++)
			{
				m_PatternedSprites[i] = CombineTextures(Patterns[i]);
			}
		}

		private Sprite CombineTextures(int[] TypeIndex)
		{
			int[] fixedArray = new int[4];
			fixedArray[0] = TypeIndex[2];
			fixedArray[1] = TypeIndex[3];
			fixedArray[2] = TypeIndex[0];
			fixedArray[3] = TypeIndex[1];

			Color[][] texs = new Color[4][];
			for (int i = 0; i < 4; i++)
			{

				int x = (int)Segments[fixedArray[i], i].rect.x;
				int y = (int)Segments[fixedArray[i], i].rect.y;
				int w = (int)Segments[fixedArray[i], i].rect.width;
				int h = (int)Segments[fixedArray[i], i].rect.height;
				texs[i] = Segments[fixedArray[i], i].texture.GetPixels(x, y, w, h);
			}

			int width_half = (int)Segments[0, 0].rect.width;
			int height_half = (int)Segments[0, 0].rect.height;
			int width = width_half * 2;
			int height = height_half * 2;

			Color[] texArray = new Color[width * height];
			for (int i = 0; i < height_half; i++)
			{
				Array.Copy(texs[0], i * width_half, texArray, i * width, width_half);
			}

			for (int i = 0; i < height_half; i++)
			{
				Array.Copy(texs[1], i * width_half, texArray, i * width + width_half, width_half);
			}

			for (int i = 0; i < height_half; i++)
			{
				Array.Copy(texs[2], i * width_half, texArray, (i + height_half) * width, width_half);
			}

			for (int i = 0; i < height_half; i++)
			{
				Array.Copy(texs[3], i * width_half, texArray, (i + height_half) * width + width_half, width_half);
			}
			Texture2D ret = new Texture2D(width, height, TextureFormat.ARGB32, false);
			ret.filterMode = FilterMode.Point;
			ret.wrapMode = TextureWrapMode.Clamp;
			ret.SetPixels(texArray);
			return Sprite.Create(ret, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), width);
		}



#if UNITY_EDITOR
		[MenuItem("Assets/Create/Custom Auto Tile")]
		public static void CreateTerrainTile()
		{
			string path = EditorUtility.SaveFilePanelInProject("Save Custom Auto Tile", "New Custom Auto Tile", "asset", "Save Custom Auto Tile", "Assets");

			if (path == "")
				return;

			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<CustomAutoTile>(), path);
		}

#endif
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(CustomAutoTile))]
	public class CustomAutoTileEditor : Editor
	{


		private CustomAutoTile tile { get { return (target as CustomAutoTile); } }

		public void OnEnable()
		{
			if (tile.m_RawTilesSprites == null || tile.m_RawTilesSprites.Length != 15)
			{
				tile.m_RawTilesSprites = new Sprite[15];
				EditorUtility.SetDirty(tile);
			}
			if (tile.m_RawTilesSprites[0] && tile.m_RawTilesSprites[1] && tile.m_RawTilesSprites[2] && tile.m_RawTilesSprites[3] && tile.m_RawTilesSprites[4])
			{
				tile.GeneratePatterns();
			}
		}


		public override void OnInspectorGUI()
		{
			string[] paramNames ={
			"左上",
			"上", 
			"右上",
			"左", 
			"中央",
			"右", 
			"左下",
			"下", 
			"右下",
			"角左上",
			"角右上",
			"角左下",
			"角右下",
			"独立"
			};
			EditorGUILayout.LabelField("順規格のオートタイルチップを上から順番にスロットしてください。（アニメーション非対応）");
			EditorGUILayout.Space();

			float oldLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 210;

			EditorGUI.BeginChangeCheck();
			for (int i = 0; i < paramNames.Length; i++)
			{
				//Debug.Log(i);

				tile.m_RawTilesSprites[i] = (Sprite)EditorGUILayout.ObjectField("("+i.ToString()+")"+paramNames[i], tile.m_RawTilesSprites[i], typeof(Sprite), false, null);

			}
			if (EditorGUI.EndChangeCheck())
			{
				if (tile.m_RawTilesSprites[0] && tile.m_RawTilesSprites[1] && tile.m_RawTilesSprites[2] && tile.m_RawTilesSprites[3] && tile.m_RawTilesSprites[4])
				{
					tile.GeneratePatterns();
				}

				EditorUtility.SetDirty(tile);
			}

			EditorGUIUtility.labelWidth = oldLabelWidth;
		}

		// Preview
		public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
		{
			if (tile.m_RawTilesSprites[13] != null)
			{

				Type t = GetType("UnityEditor.SpriteUtility");
				if (t != null)
				{
					MethodInfo method = t.GetMethod("RenderStaticPreview", new Type[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) });
					if (method != null)
					{
						object ret = method.Invoke("RenderStaticPreview", new object[] { tile.m_RawTilesSprites[13], Color.white, width, height });
						if (ret is Texture2D)
							return ret as Texture2D;
					}
				}
			}
			return base.RenderStaticPreview(assetPath, subAssets, width, height);
		}
        private static Type GetType(string TypeName)
        {
            var type = Type.GetType(TypeName);
            if (type != null)
                return type;

            if (TypeName.Contains("."))
            {
                var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));
                var assembly = Assembly.Load(assemblyName);
                if (assembly == null)
                    return null;
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }

            var currentAssembly = Assembly.GetExecutingAssembly();
            var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
            foreach (var assemblyName in referencedAssemblies)
            {
                var assembly = Assembly.Load(assemblyName);
                if (assembly != null)
                {
                    type = assembly.GetType(TypeName);
                    if (type != null)
                        return type;
                }
            }
            return null;
        }
	}
#endif
}