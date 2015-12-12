using System.Dynamic;
using AliCloudOpenSearch.com.API.Modal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.IO;
using System.Web;
using Newtonsoft.Json.Linq;

namespace AliCloudOpenSearch.com.API
{
    /// <summary>
    /// Provides aliyun opensearch document management functions
    /// </summary>
    public class CloudsearchDoc
    {
        /// <summary>
        /// The operation queue
        /// </summary>
        private List<Dictionary<String, Object>> _batchExecuteCachedDocs = new List<Dictionary<String, Object>>();

        private const String PUSH_RETURN_STATUS_OK = "OK";
        private const int MaxTryTimes = 3;


        /**
         * 在切割一个大数据块后push数据的频率。默认 5次/s。
         * @var int
         */
        private const Int32 PUSH_FREQUENCE = 4;


        /**
       * POST一个文件，进行切割时的单请求的最大size。单位：MB。
       * @var int
       */
        private const Int32 PUSH_MAX_SIZE = 4;

        /**
         * Ha3Doc文件doc分割符。
         * @var String
         */
        private const char HA_DOC_ITEM_SEPARATOR = '\x1E';

        /**
         * Ha3Doc文件字段分割符
         * @var String
         */
        private const char HA_DOC_FIELD_SEPARATOR = '\x1F';

        /**
         * Ha3Doc文件字段多值分割符。
         * @var String
         */
        private const char HA_DOC_MULTI_VALUE_SEPARATOR = '\x1D';

        /**
         * section weight标志符。
         * @var String
         */
        private const char HA_DOC_SECTION_WEIGHT = '\x1C';

        /**
           * 统计的应用名称。                                          
           * @var String
           */
        private String indexName;

        /**
         * CloudsearchClient 实例。                                  
         * @var CloudsearchClient                                    
         */
        private CloudsearchApi client;

        /**
         * 指定API接口的相对路径。                          
         * @var String                                               
         */
        private String path;

        /**
         * 构造函数。
         * @param String indexName 指定统计信息的应用名称。         
         * @param CloudsearchApi client 此对象由CloudsearchApi类实例化。
         */
        ///<prototype>public CloudsearchDoc(String indexName, CloudsearchApi client)</prototype>
        public CloudsearchDoc(String indexName, CloudsearchApi client)
        {
            this.indexName = indexName;
            this.client = client;
            this.path = "/index/doc/" + indexName;

        }

        /**
         * 根据doc id获取doc的详细信息。
         * @param String pkField 指定的字段名称。
         * @param String docId 指定的doc id。
         * @return String 
         */
        ///<prototype>public String detail(String pkField,String docId)</prototype>
        public Response Detail(String pkField, String docId)
        {
            Dictionary<String, Object> parameters = new Dictionary<String, Object>();
            parameters.Add(pkField, docId);
            return JsonConvert.DeserializeObject<Response>(this.client.ApiCall(this.path, parameters, "POST"));
        }


        /**
         * 操作docs。
         * 
         * @param array|string $docs 此docs为用户push的数据，此字段为json_encode的字符串或者
         * 数据。
         * @param String $tableName 操作的表名。
         * @param String $action 操作符，有ADD、UPDATE、REMOVE。
         * @return String 请求API并返回相应的结果。
         */
        private Response doAction(List<Dictionary<String, Object>> docs, String tableName)
        {

            Dictionary<String, Object> parameters = new Dictionary<String, Object>();
            parameters.Add("action", "push");
            parameters.Add("items", JsonConvert.SerializeObject(docs));
            parameters.Add("table_name", tableName);

            var rawRet = this.client.ApiCall(this.path, parameters, "POST");
            return JsonConvert.DeserializeObject<Response>(rawRet);
        }

        /// <summary>
        /// Batch submit the operation queue which added by add(),delete(),upadte()
        /// </summary>
        /// <param name="tableName"></param>
        public Response Push(string tableName)
        {
            var ret = doAction(_batchExecuteCachedDocs, tableName);

            if (ret.Status != "OK")
            {
                for (int i = 0; i < MaxTryTimes; i++)
                {
                    ret = doAction(_batchExecuteCachedDocs, tableName);
                    if (ret.Status == "OK")
                    {
                        break;
                    }
                }
            }

            _batchExecuteCachedDocs.Clear();
            return ret;
        }

        /// <summary>
        /// add field to update queue, will be submitted to server untill push()
        /// </summary>
        /// <param name="fields">The fields will be updated</param>
        public CloudsearchDoc Update(Dictionary<String, Object> fields)
        {
            addToQueue(fields, "update");
            return this;
        }

        /// <summary>
        /// add field to update queue, will be submitted to server untill push()
        /// </summary>
        /// <param name="fieldsList">The fields will be updated</param>
        public CloudsearchDoc Update(IList<Dictionary<String, Object>> fieldsList)
        {
            addToQueue(fieldsList, "update");
            return this;
        }

        /// <summary>
        /// add fiedls to queue, it will be submitted to server untill push()
        /// </summary>
        /// <param name="docs">added docs</param>
        public CloudsearchDoc Add(Dictionary<String, Object> fields)
        {
            addToQueue(fields, "add");
            return this;
        }

        /// <summary>
        /// Add fields to queue
        /// </summary>
        /// <param name="fields">fieds with json format,eg:[{id:1,content:"a"},{id:2,content:"b"}]</param>
        public CloudsearchDoc Add(string fields)
        {
            addToQueue(JsonConvert.DeserializeObject<Dictionary<string, object>[]>(fields), "add");
            return this;
        }

        private void addToQueue(Dictionary<String, Object> fields, string command)
        {
            Utilities.Guard(fields);
            Utilities.Guard(command);
            var doc = new Dictionary<string, Object>();
            doc["cmd"] = command;
            doc["fields"] = fields;
            _batchExecuteCachedDocs.Add(doc);
        }

        private void addToQueue(IList<Dictionary<String, Object>> fieldsList, string command)
        {
            Utilities.Guard(fieldsList);
            Utilities.Guard(command);
            foreach (var fields in fieldsList)
            {
                var doc = new Dictionary<string, Object>();
                doc["cmd"] = command;
                doc["fields"] = fields;
                _batchExecuteCachedDocs.Add(doc);
            }
        }

        /// <summary>
        /// add fiedls to cache, it will be submitted to server untill push()
        /// </summary>
        /// <param name="docs">added docs</param>
        public CloudsearchDoc Add(IList<Dictionary<String, Object>> fieldsList)
        {
            addToQueue(fieldsList, "add");
            return this;
        }


        /// <summary>
        /// Add the doc ids which will be deleted untill push()
        /// </summary>
        /// <param name="docIds">Doc ids which will be deleted</param>
        public CloudsearchDoc Remove(params string[] docIds)
        {
            Utilities.Guard(docIds);

            foreach (var docId in docIds)
            {
                var doc = new Dictionary<string, Object>();
                doc["cmd"] = "delete";
                doc["fields"] = new { id = docId };

                _batchExecuteCachedDocs.Add(doc);
            }
            return this;
        }


        ///<prototype> public Dictionary&lt;String, String&gt; pushHADocFile(String fileName, String tableName, Int32 offset = 1,Int32 maxSize = PUSH_MAX_SIZE, Int32 frequence = PUSH_FREQUENCE)</prototype>
        public Dictionary<String, String> PushHADocFile(String fileName, String tableName, Int32 offset = 1,
      Int32 maxSize = PUSH_MAX_SIZE, Int32 frequence = PUSH_FREQUENCE)
        {

            StreamReader reader = this._connect(fileName);

            // 默认doc初始结构。
            Dictionary<String, Object> doc = new Dictionary<String, Object>();
            doc["cmd"] = "";
            doc["fields"] = new Dictionary<String, Object>();

            //$doc = array('cmd' => '', 'fields' => array());

            // 当前行号，用来记录当前已经解析到了第多少行。
            Int32 lineNumber = 1;

            // 最新成功push数据的行号，用于如果在重新上传的时候设定offset偏移行号。
            Int32 lastLineNumber = 0;

            // 最后更新的doc中的字段名，如果此行没有字段结束符，则下行的数据会被添加到这行的字段上。
            // 有一些富文本，在当前行没有结束此字段，则要记录最后的字段名称。
            // 例如：
            // rich_text=鲜花
            // 礼品专卖店^_
            // other_field=xxx^_
            String lastField = "";

            // 当前还未上传的文档的大小。单位MB.
            Int32 totalSize = 0;

            // 当前秒次已经发了多少次请求，用于限流。
            Int32 timeFrequence = 0;

            // 开始遍历文件。
            String line = String.Empty;
            List<Dictionary<String, Object>> buffer = new List<Dictionary<string, object>>();

            DateTime uTime = new DateTime(1970, 1, 1, 0, 0, 0);
            Double time = 0;
            String key = "";
            String value = "";

            while ((line = reader.ReadLine()) != null)
            {

                // 如果当前的行号小于设定的offset行号时跳过。
                if (lineNumber < offset)
                {
                    continue;
                }

                // 获取结果当前行的最后两个字符。
                char[] bytes = line.ToCharArray();
                char separator = bytes[bytes.Length - 1];


                // 如果当前结束符是文档的结束符^^\n，则当前doc解析结束。并计算buffer+当前doc文档的
                // 大小，如果大于指定的文档大小，则push buffer到api，并清空buffer，同时把当前doc
                // 文档扔到buffer中。
                if (separator == HA_DOC_ITEM_SEPARATOR)
                {

                    lastField = String.Empty;


                    // 获取当前文档生成json并urlencode之后的size大小。

                    String json = JsonConvert.SerializeObject(doc);
                    Int32 currentSize = HttpUtility.UrlEncode(json).Length;

                    // 如果计算的大小+buffer的大小大于等于限定的阀值self::PUSH_MAX_SIZE，则push
                    // buffer数据。
                    if (currentSize + totalSize >= maxSize * 1024 * 1024)
                    {

                        // push 数据到api。
                        var rel = this.doAction(buffer, tableName);


                        if ("OK" != rel.Status)
                        {
                            //TODO what is this error?
                            throw new Exception("Api returns error: " +
                                ". Latest successful posted line is" + lastLineNumber.ToString());
                        }
                        else
                        {
                            // 如果push成功，则计算每秒钟的push的频率并如果超过频率则sleep。
                            lastLineNumber = lineNumber;

                            Double newTime = (DateTime.UtcNow - uTime).TotalSeconds;
                            timeFrequence++;

                            // 如果时间为上次的push时间且push频率超过设定的频率，则unsleep 剩余的毫秒数。
                            if (Math.Floor(newTime) == time && timeFrequence >= frequence)
                            {
                                Double left = Math.Floor(newTime) + 1 - newTime;
                                System.Threading.Thread.Sleep(Convert.ToInt32(left));
                                timeFrequence = 0;
                            }
                            // 重新设定时间和频率。
                            newTime = (DateTime.UtcNow - uTime).TotalSeconds;
                            if (time != newTime)
                            {

                                time = newTime;
                                timeFrequence = 0;
                            }
                        }

                        // 重置buffer为空，并重新设定total size 为0； 
                        buffer.Clear();
                        totalSize = 0;
                    }
                    // doc 添加到buffer中，并增加total size的大小。
                    buffer.Add(doc);
                    totalSize += currentSize;

                    doc = new Dictionary<string, object>();
                    // 初始化doc。
                    doc["cmd"] = "";
                    doc["fields"] = new Dictionary<String, Object>();

                }
                else if (separator == HA_DOC_FIELD_SEPARATOR)
                {
                    // 表示当前字段结束。

                    String detail = line.TrimEnd(new char[] { HA_DOC_FIELD_SEPARATOR, '\n' });


                    if (!String.IsNullOrEmpty(lastField))
                    {

                        // 表示当前行非第一行数据，则获取最后生成的字段名称并给其赋值。
                        Dictionary<String, Object> item = doc["fields"] as Dictionary<String, Object>;

                        String[] fv = this._extractFieldValue(item[lastField] + detail);
                        if (fv.Length == 1)
                        {
                            item[lastField] = fv[0];
                        }
                        else
                        {
                            item[lastField] = fv;
                        }


                    }
                    else
                    {

                        // 表示当前为第一行数据，则解析key 和value。
                        try
                        {
                            List<String> data = this._parseHADocField(detail);
                            key = data[0];
                            value = data[1];
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message +
                                ". Latest successful posted line number is " + lastLineNumber);
                        }

                        if (key.ToUpper() == "CMD")
                        {
                            doc["cmd"] = value.ToUpper();
                        }
                        else
                        {
                            Dictionary<String, Object> item = doc["fields"] as Dictionary<String, Object>;

                            String[] fv = this._extractFieldValue(value);
                            if (fv.Length == 1)
                            {
                                item[key] = fv[0];
                            }
                            else
                            {
                                item[key] = fv;
                            }

                        }
                    }

                    // 设置字段名称为空。
                    lastField = "";
                }
                else
                {
                    // 此else 表示富文本的非最后一行。
                    line = line + "\n";
                    // 表示富文本非第一行。
                    if (!String.IsNullOrEmpty(lastField))
                    {
                        Dictionary<String, Object> item = doc["fields"] as Dictionary<String, Object>;
                        item[lastField] += line;

                    }
                    else
                    {
                        // 表示字段的第一行数据。
                        try
                        {
                            List<String> data = this._parseHADocField(line);
                            key = data[0];
                            value = data[1];

                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message +
                                ". Latest successful posted line number is " + lastLineNumber);
                        }

                        Dictionary<String, Object> item = doc["fields"] as Dictionary<String, Object>;
                        item[key] = value;
                        lastField = key;
                    }
                }
                lineNumber++;
            }

            reader.Close();
            // 如果buffer 中还有数据则再push一次数据。
            if (buffer.Count > 0)
            {
                // push 数据到api。

                var rel1 = this.doAction(buffer, tableName);

                if (PUSH_RETURN_STATUS_OK != rel1.Status)
                {
                    throw new Exception("Api returns error: " +
                        ". Latest successful posted line number is " + lastLineNumber);
                }
            }

            return new Dictionary<string, string>()
            {
                {"status", "OK"},
                {"message", "The data is posted successfully."}
            };

        }

        /**
         * 创建一个文件指针资源。
         * @param String $fileName
         * @throws Exception
         * @return resource 返回文件指针。
         */
        private StreamReader _connect(String fileName)
        {
            StreamReader mysr = new StreamReader(fileName, Encoding.UTF8);
            return mysr;
        }

        /**
         * 解析一段字符串并生成key和value。
         * @param String $string
         * @return String|boolean 返回一个数组有两个字段，第一个为key，第二个为value。如果解析
         * 失败则返回错误。
         */
        private List<String> _parseHADocField(String str)
        {
            str = str.Substring(0, str.Length);
            String separater = "=";
            Int32 pos = str.IndexOf(separater);
            //Dictionary<String,Object> rel = new Dictionary<string,object>();
            List<String> rel = new List<String>();
            if (pos != -1)
            {
                String key = str.Substring(0, pos);
                String value = str.Substring(pos + 1);
                rel.Add(key);
                rel.Add(value);
                return rel;
            }
            else
            {
                throw new Exception("The are no key and value in the field.");
            }
        }

        /**
         * 检查字段值的值是否为多值字段，如果是则返回多值的数组，否则返回一个string的结果。
         * @param String $value 需要解析的结果。
         * @return String|string 如果非多值则返回字符串，否则返回多值数组。
         */
        private String[] _extractFieldValue(String value)
        {
            char[] sep = new char[] { HA_DOC_MULTI_VALUE_SEPARATOR };
            String[] split = value.Split(sep, StringSplitOptions.None);
            return split;
        }

    }
}
