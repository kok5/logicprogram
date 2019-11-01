using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapEditor
{
    public class UIPanelProperty2 : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        public static UIPanelProperty2 ins = null;

        private RectTransform curRecTran;
        private Vector3 offsetPos = Vector3.zero;


        //-----------------------------------------------------------------
        //base
        //层
        private Slider sliderLayer;
        private Text txtLayer;
        //左右翻转
        private Toggle toggleLeftRight;
        //上下翻转 
        private Toggle toggleUpDown;
        //初始生效 
        private Toggle toggleValid;
        //初始可见 
        private Toggle toggleVisible;

        //-----------------------------------------------------------------
        //物理
        //质量
        private Slider sliderMass;
        private Text txtMass;
        //弹性 
        private Slider sliderBounciness;
        private Text txtBounciness;
        //重力
        private Toggle toggleGravity;
        //碰撞
        private Toggle toggleCollision;

        //-----------------------------------------------------------------
        //自定义参数
        //质量
        private Slider sliderParameter;
        private Text txtParameter;

        //参数有效性
        private Toggle toggleParameterValid;
        //参数选择
        private Dropdown dropDownParameterList;
        //初始可见
        private GameObject componentObj;

        //===================================================
        //
        private FieldInfo[] fields;
        private int filedIndex = 0;

        public void InitData(GameObject objRoot)
        {
            componentObj = objRoot;

            if (objRoot != null)
            {
                CustomerPropertyBase com = objRoot.GetComponent<CustomerPropertyBase>();
                if (com != null)
                {
                    Type type = com.GetType();

                    FieldInfo field = type.GetField("serialization");
                    if (field != null)
                    {
                        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

                        Debug.Log("field.FieldType: " + field.FieldType.ToString());

                        fields = Reflection.GetSerializableFields(field.FieldType);

                        var value = field.GetValue(com);
                        for (int i = 0; i < fields.Length; ++i)
                        {
                            MemberInfo memberInfo = fields[i];
                            Type type1 = fields[i].FieldType;
                            //Debug.Log("sub type name: " + type1.ToString());
                            
                            Debug.Log("Filed Name: " + memberInfo.Name);
                            Debug.Log("Filed Value: " + fields[i].GetValue(value));

                            var v = (DescriptionAttribute[])memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                            var descriptionName = v[0].Description;

                            //options.Add(new Dropdown.OptionData(memberInfo.Name));
                            options.Add(new Dropdown.OptionData(descriptionName));
                        }

                        dropDownParameterList.options = options;
                    }
                    else
                    {
                        dropDownParameterList.gameObject.SetActive(false);
                    }
                }

            }
        }

        private void OnSelectedParameterChanged(int index)
        {
            print("Parameter Select index: " + index);
            filedIndex = index;

            FieldInfo fi = fields[filedIndex];
            if (fi.GetType() == typeof(System.Boolean))
            {
                sliderParameter.gameObject.SetActive(false);
                txtParameter.gameObject.SetActive(false);
            }
            else
            {
                sliderParameter.gameObject.SetActive(true);
                txtParameter.gameObject.SetActive(true);

                if (componentObj != null)
                {
                    CustomerPropertyBase com = componentObj.GetComponent<CustomerPropertyBase>();
                    if (com != null)
                    {
                        Type type = com.GetType();
                        FieldInfo field = type.GetField("serialization");
                        if (field != null)
                        {
                            var value = field.GetValue(com);

                            var subFiledValue = fi.GetValue(value);


                            if (fi.FieldType == typeof(System.Int32))
                            {
                                txtParameter.gameObject.SetActive(true);
                                sliderParameter.gameObject.SetActive(true);
                                sliderParameter.value = (int)subFiledValue;
                                txtParameter.text = subFiledValue.ToString();
                            }
                            else if (fi.FieldType == typeof(System.Single))
                            {
                                txtParameter.gameObject.SetActive(true);
                                sliderParameter.gameObject.SetActive(true);
                                sliderParameter.value = (float)subFiledValue;
                                txtParameter.text = subFiledValue.ToString();
                            }
                            else
                            {
                                txtParameter.gameObject.SetActive(false);
                                sliderParameter.gameObject.SetActive(false);
                            }


                        }
                    }
                }

            }
        }

        private void OnParameterChanged(float value)
        {
            if (txtParameter != null)
                txtParameter.text = value.ToString();

            FieldInfo fi = fields[filedIndex];
            if ((fi.FieldType == typeof(System.Int32)) || (fi.FieldType == typeof(System.Single)))
            {
                if (componentObj != null)
                {
                    CustomerPropertyBase com = componentObj.GetComponent<CustomerPropertyBase>();
                    if (com != null)
                    {
                        Type type = com.GetType();
                        FieldInfo field = type.GetField("serialization");
                        if (field != null)
                        {
                            var rootValue = field.GetValue(com);

                            if (fi.FieldType == typeof(System.Int32))
                                fi.SetValue(rootValue, (int)value);
                            else
                                fi.SetValue(rootValue, value);

                            Debug.Log("@@@@@subFiledValue setNewValue: " + value);

                        }
                    }
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector3 globalMousePos;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(curRecTran, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                offsetPos = curRecTran.position - globalMousePos;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 globalMousePos;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(curRecTran, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                curRecTran.position = globalMousePos + offsetPos;
            }
        }

        void Start()
        {

        }


        public void OnLayerChange(float value)
        {
            Debug.Log("new layer index: " + value);

            if (txtLayer != null)
                txtLayer.text = value.ToString();
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        void Awake()
        {
            ins = this;

            curRecTran = transform.GetComponent<RectTransform>();

            //层
            sliderLayer = transform.Find("body/Scroll_View_1/Viewport/content/basic/SliderLayer").GetComponent<Slider>();

            if (sliderLayer != null)
            {
                sliderLayer.minValue = 1;
                sliderLayer.maxValue = EditorLayerMgr.MAX_LAYER_COUNT;

                sliderLayer.onValueChanged.AddListener(OnLayerChange);
            }

            txtLayer = transform.Find("body/Scroll_View_1/Viewport/content/basic/txtLayer").GetComponent<Text>();
            //左右翻转
            toggleLeftRight = transform.Find("body/Scroll_View_1/Viewport/content/basic/ToggleLeftRight").GetComponent<Toggle>();
            //上下翻转 
            toggleUpDown = transform.Find("body/Scroll_View_1/Viewport/content/basic/ToggleUpDown").GetComponent<Toggle>();
            //初始生效
            toggleValid = transform.Find("body/Scroll_View_1/Viewport/content/basic/ToogleValid").GetComponent<Toggle>();
            //
            toggleVisible = transform.Find("body/Scroll_View_1/Viewport/content/basic/ToggleVisilbe").GetComponent<Toggle>();

            ////-----------------------------------------------------------------
            //物理

            sliderMass = transform.Find("body/Scroll_View_1/Viewport/content/physic/SliderMass").GetComponent<Slider>();
            txtMass = transform.Find("body/Scroll_View_1/Viewport/content/physic/txtMass").GetComponent<Text>();
            ////弹性 
            sliderBounciness = transform.Find("body/Scroll_View_1/Viewport/content/physic/SliderBounciness").GetComponent<Slider>();
            txtBounciness = transform.Find("body/Scroll_View_1/Viewport/content/physic/txtBounciness").GetComponent<Text>();
            ////重力
            toggleGravity = transform.Find("body/Scroll_View_1/Viewport/content/physic/ToggleGravity").GetComponent<Toggle>();
            ////碰撞
            toggleCollision = transform.Find("body/Scroll_View_1/Viewport/content/physic/ToggleCollision").GetComponent<Toggle>();

            //-----------------------------------------------------------------
            //自定义参数

            sliderParameter = transform.Find("body/Scroll_View_1/Viewport/content/customproperty/SliderParameter").GetComponent<Slider>();
            txtParameter = transform.Find("body/Scroll_View_1/Viewport/content/customproperty/txtParameter").GetComponent<Text>();
            if (sliderParameter != null)
            {
                sliderParameter.minValue = 1;
                sliderParameter.maxValue = 200;

                sliderParameter.onValueChanged.AddListener(OnParameterChanged);
            }

            ////参数有效性
            toggleParameterValid = transform.Find("body/Scroll_View_1/Viewport/content/customproperty/ToggleParameterValid").GetComponent<Toggle>();
            ////参数选择
            dropDownParameterList = transform.Find("body/Scroll_View_1/Viewport/content/customproperty/Dropdown").GetComponent<Dropdown>();

            dropDownParameterList.onValueChanged.AddListener(OnSelectedParameterChanged);
        }

        void OnDestroy()
        {
            if (dropDownParameterList != null)
            {
                dropDownParameterList.onValueChanged.RemoveListener(OnSelectedParameterChanged);
            }
        }



    }
}