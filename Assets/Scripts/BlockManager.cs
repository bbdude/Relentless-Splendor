using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockManager : MonoBehaviour 
{

	List<Block> blocks = new List<Block>();
  public GameObject pref;
  public int selBlock;
  
  List<Texture2D> breaking = new List<Texture2D>();
  
  void Start()
  {
    List<Vector3> tempLocs = new List<Vector3>()
    { new Vector3(-8.499073f, -3.705792f, 0), new Vector3(-7.499073f, -3.705792f, 0), new Vector3(-6.499073f, -3.705792f, 0), new Vector3(-5.499073f, -3.705792f, 0), new Vector3(-4.499073f, -3.705792f, 0), new Vector3(-3.499073f, -3.705792f, 0), new Vector3(-2.499073f, -3.705792f, 0), new Vector3(-1.499073f, -3.705792f, 0), new Vector3(-0.499073f, -3.705792f, 0), new Vector3(0.5009267f, -3.705792f, 0), new Vector3(1.5009267f, -3.705792f, 0), new Vector3(2.5009267f, -3.705792f, 0), new Vector3(3.5009267f, -3.705792f, 0), new Vector3(4.5009267f, -3.705792f, 0), new Vector3(5.5009267f, -3.705792f, 0), new Vector3(6.5009267f, -3.705792f, 0)
    , new Vector3(-8.499073f, -5.705792f, 0), new Vector3(-7.499073f, -5.705792f, 0), new Vector3(-6.499073f, -5.705792f, 0), new Vector3(-5.499073f, -5.705792f, 0), new Vector3(-4.499073f, -5.705792f, 0), new Vector3(-3.499073f, -5.705792f, 0), new Vector3(-2.499073f, -5.705792f, 0), new Vector3(-1.499073f, -5.705792f, 0), new Vector3(-0.499073f, -5.705792f, 0), new Vector3(0.5009267f, -5.705792f, 0), new Vector3(1.5009267f, -5.705792f, 0), new Vector3(2.5009267f, -5.705792f, 0), new Vector3(3.5009267f, -5.705792f, 0), new Vector3(4.5009267f, -5.705792f, 0), new Vector3(5.5009267f, -5.705792f, 0), new Vector3(6.5009267f, -5.705792f, 0)
    , new Vector3(1.500927f, -2.71f, 0), new Vector3(2.500927f, -2.71f, 0), new Vector3(3.500927f, -2.71f, 0), new Vector3(4.500927f, -2.71f, 0), new Vector3(2.500927f, -1.71f, 0) 
    };
    
    foreach(Vector3 vec in tempLocs)
    {
      Block tempobj;
      if (vec.y > -5)
        tempobj = new Block(0, "Grass", vec.x, vec.y);
      else
        tempobj = new Block(0, "Wall", vec.x, vec.y);
      
      tempobj.obj = (GameObject)Instantiate(pref, tempobj.pos, Quaternion.identity);
      
      Renderer rend = tempobj.obj.GetComponent<Renderer>();
      if (rend != null){
        rend.material = tempobj._thisMat;
      }
      
      blocks.Add(tempobj);
    }
  }
  
  public int OnHold(GameObject id, Vector3 pos)
  {
    int i = 0;
    bool stopCounting = false;
    Rect area = new Rect(pos.x - 5, pos.y - 5, 10, 10);
    foreach(Block block in blocks)
    {
      if (id == block.obj && area.Contains(block.pos))
      {
        block.AssignBreak();
        block.held = true;
        stopCounting = true;
        block.hardness -= 1;
        if (block.hardness <= 0)
        {
          Destroy(blocks[i].obj);
          blocks.Remove(blocks[i]);
          selBlock = -1;
          return -1;
        }
      }
      else if (block.held)
      {
        block.held = false;
        block.SwapMats();
      }
      if (!stopCounting)
        i++;
    }
    
    if (stopCounting)
    {
      selBlock = i;
      return i; 
    }
    else
    {
      selBlock = -1;
      return 0;
    }
  }
  
  public void OnRelease()
  {
    if (selBlock == -1)
      return;
    blocks[selBlock].held = false;
    blocks[selBlock].SwapMats();
  }
  
}

class Block
{
  public int _id;
  public string _type;
  public Material _thisMat;
  public Material _breakMat;
  public GameObject obj;
  public Vector3 pos;
  public bool held = false;
  public int hardness = 10;
  
  public Block(int id, string type, float x, float y)
  {
    _id = id;
    _type = type;
    
    string folder = "Blocks/Materials/" + type + "Mat";
    _thisMat = (Material)(Resources.Load(folder) as Material);
    
    folder = "Blocks/Materials/BreakMat";
    _breakMat = (Material)(Resources.Load(folder) as Material);
    
    pos = new Vector3(x, y, 0);
    
    switch(type)
    {
      case "Grass":
        hardness = 10;
        break;
      case "Wood":
        hardness = 20;
        break;
      case "Wall":
        hardness = 100000000;
        break;
    }
  }
  
  public void SwapMats()
  {
    if (held)
    {
      Renderer rend = obj.GetComponent<Renderer>();
      if (rend != null){
        rend.material = _breakMat;
      }
    }
    else
    {
      
      Renderer rend = obj.GetComponent<Renderer>();
      if (rend != null){
        rend.material = _thisMat;
      }
    }
  }
  
  public void AssignBreak()
  {
    Renderer rend = obj.GetComponent<Renderer>();
    if (rend != null){
      rend.material = _breakMat;
    }
  }
}
