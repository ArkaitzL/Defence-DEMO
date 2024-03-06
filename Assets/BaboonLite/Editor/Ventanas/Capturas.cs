using UnityEngine;
using UnityEditor;

public class Capturas : EditorWindow
{
    [MenuItem("BaboonLite/Capturar")]
    private static void CapturarVistaPrevia()
    {
        // Selecciona el prefab en la ventana de proyectos
        GameObject prefab = Selection.activeObject as GameObject;

        if (prefab != null)
        {
            // Captura la vista previa del prefab
            Texture2D vistaPrevia = AssetPreview.GetAssetPreview(prefab);

            // Guarda la vista previa como una textura en la misma carpeta que el prefab
            if (vistaPrevia != null)
            {
                string prefabPath = AssetDatabase.GetAssetPath(prefab);
                string prefabFolder = System.IO.Path.GetDirectoryName(prefabPath);
                string prefabName = System.IO.Path.GetFileNameWithoutExtension(prefabPath);

                // Ajusta la ruta del archivo de la captura para que esté en la misma carpeta y con el mismo nombre
                string rutaArchivo = prefabFolder + "/" + prefabName + ".png";

                // Guarda la captura en la nueva ruta
                byte[] bytes = vistaPrevia.EncodeToPNG();
                System.IO.File.WriteAllBytes(rutaArchivo, bytes);

                // Refresca el proyecto para que se muestre la nueva captura
                AssetDatabase.Refresh();

                Debug.Log("Captura creada en: " + rutaArchivo);
            }
            else
            {
                Debug.LogWarning("No se pudo obtener la vista previa del prefab.");
            }
        }
        else
        {
            Debug.LogWarning("Selecciona un prefab en la ventana de proyectos.");
        }
    }
}
