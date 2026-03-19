package rabbit

import (
	"log"
	amqp "github.com/rabbitmq/amqp091-go"
)

type Consumer struct {
	conn    *amqp.Connection
	channel *amqp.Channel
}

func NewConsumer(url string) *Consumer {
	conn, err := amqp.Dial(url)
	if err != nil {
		log.Fatal(err)
	}

	ch, err := conn.Channel()
	if err != nil {
		log.Fatal(err)
	}

	_, err = ch.QueueDeclare(
		"orchestration.jobs",
		true,
		false,
		false,
		false,
		nil,
	)

	if err != nil {
		log.Fatal(err)
	}

	return &Consumer{
		conn:    conn,
		channel: ch,
	}
}

func (c *Consumer) Consume(handler func([]byte)) {
	msgs, err := c.channel.Consume(
		"orchestration.jobs",
		"",
		true,
		false,
		false,
		false,
		nil,
	)

	if err != nil {
		log.Fatal(err)
	}

	for msg := range msgs {
		handler(msg.Body)
	}
}
