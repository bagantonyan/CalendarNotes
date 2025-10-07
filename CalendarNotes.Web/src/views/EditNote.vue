<template>
  <div style="max-width: 600px; margin: 0 auto">
    <n-card title="Редактировать заметку">
      <n-form v-if="note" :model="formValue">
        <n-form-item label="Заголовок">
          <n-input v-model:value="formValue.title" />
        </n-form-item>
        <n-form-item label="Текст">
          <n-input v-model:value="formValue.text" type="textarea" :rows="5" />
        </n-form-item>
        <n-form-item label="Время уведомления">
          <n-date-picker v-model:value="notificationTime" type="datetime" style="width: 100%" />
        </n-form-item>
        <n-space>
          <n-button type="primary" :loading="notesStore.loading" @click="handleUpdate">
            Сохранить
          </n-button>
          <n-button @click="router.back()">Отмена</n-button>
        </n-space>
      </n-form>
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { NCard, NForm, NFormItem, NInput, NDatePicker, NButton, NSpace, useMessage } from 'naive-ui'
import { useNotesStore } from '@/stores/notes'

const router = useRouter()
const route = useRoute()
const notesStore = useNotesStore()
const message = useMessage()

const noteId = Number(route.params.id)
const note = computed(() => notesStore.notes.find((n) => n.id === noteId))
const formValue = ref({ id: noteId, title: '', text: '', notificationTime: '' })
const notificationTime = ref<number>(Date.now())

onMounted(async () => {
  // Загружаем заметки если еще не загружены
  if (notesStore.notes.length === 0) {
    await notesStore.fetchNotes()
  }
  
  // Инициализируем форму данными заметки
  if (note.value) {
    formValue.value.title = note.value.title
    formValue.value.text = note.value.text
    notificationTime.value = new Date(note.value.notificationTime).getTime()
  } else {
    // Если заметка не найдена, перенаправляем на список
    message.error('Заметка не найдена')
    router.push('/notes')
  }
})

async function handleUpdate() {
  try {
    formValue.value.notificationTime = new Date(notificationTime.value).toISOString()
    await notesStore.updateNote(formValue.value)
    message.success('Заметка обновлена!')
    router.push('/notes')
  } catch {
    message.error('Ошибка обновления')
  }
}
</script>

