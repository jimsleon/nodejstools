﻿//*********************************************************//
//    Copyright (c) Microsoft. All rights reserved.
//    
//    Apache 2.0 License
//    
//    You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//    
//    Unless required by applicable law or agreed to in writing, software 
//    distributed under the License is distributed on an "AS IS" BASIS, 
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or 
//    implied. See the License for the specific language governing 
//    permissions and limitations under the License.
//
//*********************************************************//

using System;
using Microsoft.NodejsTools.Debugger;
using Microsoft.NodejsTools.Debugger.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace NodejsTests.Debugger.Serialization
{
    [TestClass]
    public class EvaluationVariableTests
    {
        [TestMethod, Priority(0), TestCategory("Debugging")]
        public void CreateEvaluationVariable()
        {
            // Arrange
            JObject json = SerializationTestData.GetEvaluationJsonObject();
            var stackFrame = new NodeStackFrame(0);
            const string name = "name";

            // Act
            var result = new NodeEvaluationVariable(stackFrame, name, json);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(NodePropertyAttributes.None, result.Attributes);
            Assert.IsNull(result.Class);
            Assert.AreEqual(16, result.Id);
            Assert.AreEqual(name, result.Name);
            Assert.IsNull(result.Parent);
            Assert.AreEqual(stackFrame, result.StackFrame);
            Assert.AreEqual("<tag>Value</tag>", result.Text);
            Assert.AreEqual(NodePropertyType.Normal, result.Type);
            Assert.AreEqual("string", result.TypeName);
            Assert.AreEqual("<tag>Value</tag>", result.Value);
        }

        [TestMethod, Priority(0), TestCategory("Debugging")]
        public void CreateBacktraceVariableWithNullStackFrame()
        {
            // Arrange
            JObject json = SerializationTestData.GetBacktraceJsonObject();
            Exception exception = null;
            NodeEvaluationVariable result = null;
            const string name = "name";

            // Act
            try
            {
                result = new NodeEvaluationVariable(null, name, json);
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.IsNull(result);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(ArgumentNullException));
        }

        [TestMethod, Priority(0), TestCategory("Debugging")]
        public void CreateBacktraceVariableWithNullName()
        {
            // Arrange
            JObject json = SerializationTestData.GetBacktraceJsonObject();
            Exception exception = null;
            NodeEvaluationVariable result = null;
            const string name = "name";

            // Act
            try
            {
                result = new NodeEvaluationVariable(null, name, json);
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.IsNull(result);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(ArgumentNullException));
        }

        [TestMethod, Priority(0), TestCategory("Debugging")]
        public void CreateBacktraceVariableWithNullJson()
        {
            // Arrange
            var stackFrame = new NodeStackFrame(0);
            Exception exception = null;
            NodeEvaluationVariable result = null;
            const string name = "name";

            // Act
            try
            {
                result = new NodeEvaluationVariable(stackFrame, name, null);
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.IsNull(result);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(ArgumentNullException));
        }
    }
}