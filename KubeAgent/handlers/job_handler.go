package handlers

import (
	"encoding/json"
	"log"

	"github.com/icykcypher/kubelab/kube-agent/models"
	"github.com/icykcypher/kubelab/kube-agent/k8s"
)

type JobHandler struct {
	k8s *k8s.Client
}

func NewJobHandler(k *k8s.Client) *JobHandler {
	return &JobHandler{k8s: k}
}

func (h *JobHandler) Handle(body []byte) {
	var job models.OrchestrationJob

	if err := json.Unmarshal(body, &job); err != nil {
		log.Println("Invalid message:", err)
		return
	}

	log.Println("Received job:", job.Action)

	switch job.Action {

	case "DeployRevision":
		h.k8s.CreateDeployment("default", job.TargetId, "nginx")

	case "DeleteService":
		log.Println("Delete not implemented yet")

	default:
		log.Println("Unknown action:", job.Action)
	}
}
