<template>
  <div style="max-width: 1200px; margin: 0 auto">
    <n-space vertical :size="20">
      <n-space justify="space-between">
        <h1>Мои заметки</h1>
        <n-space>
          <n-button @click="notesStore.exportToCsv()">Экспорт CSV</n-button>
          <n-button type="primary" @click="router.push('/notes/create')">
            + Создать заметку
          </n-button>
        </n-space>
      </n-space>

      <n-spin :show="notesStore.loading">
        <n-empty v-if="notesStore.notes.length === 0" description="У вас пока нет заметок">
          <template #extra>
            <n-button @click="router.push('/notes/create')">Создать первую заметку</n-button>
          </template>
        </n-empty>

        <n-grid v-else :cols="3" :x-gap="20" :y-gap="20">
          <n-grid-item v-for="note in notesStore.notes" :key="note.id">
            <n-card :title="note.title" hoverable>
              <p>{{ note.text }}</p>
              <template #footer>
                <n-space vertical :size="8">
                  <n-text depth="3">
                    Уведомление: {{ formatDate(note.notificationTime) }}
                  </n-text>
                  <n-tag :type="note.isNotified ? 'success' : 'warning'" size="small">
                    {{ note.isNotified ? 'Уведомление отправлено' : 'Ожидает' }}
                  </n-tag>
                  <n-space>
                    <n-button size="small" @click="router.push(`/notes/${note.id}/edit`)">
                      Редактировать
                    </n-button>
                    <n-popconfirm @positive-click="handleDelete(note.id)">
                      <template #trigger>
                        <n-button size="small" type="error">Удалить</n-button>
                      </template>
                      Удалить эту заметку?
                    </n-popconfirm>
                  </n-space>
                </n-space>
              </template>
            </n-card>
          </n-grid-item>
        </n-grid>
      </n-spin>
    </n-space>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import {
  NSpace,
  NCard,
  NButton,
  NGrid,
  NGridItem,
  NEmpty,
  NSpin,
  NTag,
  NText,
  NPopconfirm,
  useMessage
} from 'naive-ui'
import { useNotesStore } from '@/stores/notes'

const router = useRouter()
const notesStore = useNotesStore()
const message = useMessage()

onMounted(() => {
  notesStore.fetchNotes()
})

async function handleDelete(id: number) {
  try {
    await notesStore.deleteNote(id)
    message.success('Заметка удалена')
  } catch {
    message.error('Ошибка удаления')
  }
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleString('ru-RU')
}
</script>

