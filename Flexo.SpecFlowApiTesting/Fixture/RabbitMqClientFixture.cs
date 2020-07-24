using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Flexo.SpecFlowApiTesting.Extensions;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TechTalk.SpecFlow;

namespace Flexo.SpecFlowApiTesting.Fixture
{
    public class RabbitMqClientFixture : IDisposable
    {
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;

        private BlockingCollection<JObject> collectionOfSignal;

        private ScenarioContext scenatioContext;

        public RabbitMqClientFixture(ScenarioContext scenatioContext)
        {
            this.scenatioContext = scenatioContext;
            collectionOfSignal = new BlockingCollection<JObject>();
        }

        public void StartClient(string connectionString)
        {
            if (_factory == null)
            {
                _factory = new ConnectionFactory()
                {
                    Uri = new Uri(connectionString),
                    AutomaticRecoveryEnabled = true
                };
            }

            if (_connection == null)
            {
                _connection = _factory.CreateConnection();
            }

            if (_channel == null)
            {
                _channel = _connection.CreateModel();
            }
        }

        public void SubscribeToExchange(string signal, string exchange)
        {
            var queueName = $"Testing.{exchange}.{Guid.NewGuid()}.Queue";
            var queue = _channel.QueueDeclare(queueName, false, true, true);
            _channel.QueueBind(queue, exchange, "");
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var json = JObject.Parse(message);
                collectionOfSignal.Add(json);
            };
            _channel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: consumer);
        }

        public JObject WaitForSignal(string signal, IEnumerable<string> expressions)
        {
            var timeout = 240;
            JObject result = null;
            while (timeout > 0)
            {
                var findSignal = collectionOfSignal.FirstOrDefault(s => find(s, expressions));

                if (findSignal != null)
                {
                    return findSignal;
                }
                Thread.Sleep(1000);
                timeout--;
            }
            return result;
        }

        private bool find(JObject o, IEnumerable<string> expressions)
        {
            foreach (var exp in expressions)
            {
                var jsonPath = exp.ParseList("=")[0];
                var value = exp.ParseList("=")[1];

                var seleceted = o.SelectToken(jsonPath);

                if (seleceted == null)
                {
                    if (value.Contains("null"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }

                var isMatch = seleceted.ToString().Equals(value);
                if (!isMatch)
                {
                    return false;
                }
            }

            return true;
        }

        public void PublishToExchange(string exchange, string signal)
        {
            IBasicProperties basicProperties = _channel.CreateBasicProperties();
            basicProperties.Persistent = true;
            basicProperties.Priority = 3;
            _channel.BasicPublish(exchange, "", basicProperties, Serialize(signal));
        }

        private byte[] Serialize(string signal)
        {
            return Encoding.UTF8.GetBytes(signal);
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}
