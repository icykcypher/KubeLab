package main

import (
	"log"
	"os"

	"github.com/icykcypher/kubelab/kube-agent/k8s"
	"github.com/icykcypher/kubelab/kube-agent/rabbit"
	"github.com/icykcypher/kubelab/kube-agent/handlers"
)

func main() {
	rabbitUrl := os.Getenv("RABBITMQ_URL")
	if rabbitUrl == "" {
		log.Fatal("RABBITMQ_URL not set")
	}

	consumer := rabbit.NewConsumer(rabbitUrl)
	k8sClient := k8s.NewClient()

	handler := handlers.NewJobHandler(k8sClient)

	log.Println("Orchestrator started...")

	consumer.Consume(handler.Handle)
}
