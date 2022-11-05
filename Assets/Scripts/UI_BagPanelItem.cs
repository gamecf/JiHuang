using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_BagPanelItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Image bg;
    [SerializeField] Image iconImg;
    public ItemDefine itemDefine;
    private bool isSelect = false;

    public bool IsSelect
    {
        get => isSelect;
        set
        {
            isSelect = value;
            if (isSelect)
            {
                bg.color = Color.green;
            }
            else
            {
                bg.color = Color.white;
            }
        }
    }
    private void Update()
    {
        if (IsSelect && itemDefine != null && Input.GetMouseButtonDown(1))
        {
            if (Player_Controller.Instance.UseItem(itemDefine.ItemType))
            {
                Player_Controller.Instance.PlayAudio(4);
                Init(null);
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        IsSelect = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        IsSelect = false;
    }
    /// <summary>
    /// 初始化，如果传一个null过来，相当于空格子的逻辑
    /// </summary>
    /// <param name="itemDefine"></param>
    public void Init(ItemDefine itemDefine = null)
    {
        this.itemDefine = itemDefine;
        IsSelect = false;
        if (this.itemDefine == null)
        {
            iconImg.gameObject.SetActive(false);
        }
        else
        {
            iconImg.gameObject.SetActive(true);
            iconImg.sprite = itemDefine.Icon;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemDefine == null) return;
        Player_Controller.Instance.isDarging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemDefine == null) return;
        iconImg.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemDefine == null) return;
        Player_Controller.Instance.isDarging = false;
        // 发射射线查看当前碰到的物体
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo))
        {
            string targetTag = hitInfo.collider.tag;
            iconImg.transform.localPosition = Vector3.zero; //归位
            switch (itemDefine.ItemType)
            {
                case ItemType.Meat:
                    if (targetTag == "Ground")
                    {
                        Instantiate(itemDefine.Prefab, hitInfo.point + new Vector3(0, 1f, 0), Quaternion.identity);
                        Init(null);
                    }
                    else if (targetTag == "Campfire")
                    {
                        Init(ItemManager.Instance.GetItemDefine(ItemType.CookedMeat));
                    }
                    break;
                case ItemType.CookedMeat:
                    if (targetTag == "Ground")
                    {
                        Instantiate(itemDefine.Prefab, hitInfo.point + new Vector3(0, 1f, 0), Quaternion.identity);
                        Init(null);
                    }
                    else if (targetTag == "Campfire")
                    {
                        hitInfo.collider.GetComponent<Campfire_Conllider>().AddWood();
                        Init(null);
                    }
                    break;
                case ItemType.Wood:
                    if (targetTag == "Ground")
                    {
                        Instantiate(itemDefine.Prefab, hitInfo.point + new Vector3(0, 1f, 0), Quaternion.identity);
                        Init(null);
                    }
                    else if (targetTag == "Campfire")
                    {
                        hitInfo.collider.GetComponent<Campfire_Conllider>().AddWood();
                        Init(null);
                    }
                    break;
                case ItemType.Campfire:
                    if (targetTag == "Ground")
                    {
                        Instantiate(itemDefine.Prefab, hitInfo.point, Quaternion.identity);
                        Init(null);
                    }
                    break;
            }
        }
    }
}
