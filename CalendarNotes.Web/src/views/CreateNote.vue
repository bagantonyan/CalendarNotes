<template>
  <div style="max-width: 600px; margin: 0 auto">
    <n-card title="Создать заметку">
      <n-form :model="formValue" :rules="rules">
        <n-form-item label="Заголовок" path="title">
          <n-input v-model:value="formValue.title" />
        </n-form-item>
        <n-form-item label="Текст" path="text">
          <n-input v-model:value="formValue.text" type="textarea" :rows="5" />
        </n-form-item>
        <n-form-item label="Время уведомления" path="notificationTime">
          <n-date-picker v-model:value="notificationTime" type="datetime" style="width: 100%" />
        </n-form-item>
        <n-space>
          <n-button type="primary" :loading="notesStore.loading" @click="handleCreate">
            Создать
          </n-button>
          <n-button @click="router.back()">Отмена</n-button>
        </n-space>
      </n-form>
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { NCard, NForm, NFormItem, NInput, NDatePicker, NButton, NSpace, useMessage } from 'naive-ui'
import { useNotesStore } from '@/stores/notes'

const router = useRouter()
const notesStore = useNotesStore()
const message = useMessage()

const formValue = ref({ title: '', text: '', notificationTime: '' })
const notificationTime = ref<number>(Date.now() + 3600000)
const rules = {
  title: { required: true, message: 'Введите заголовок' },
  text: { required: true, message: 'Введите текст' }
}

async function handleCreate() {
  try {
    formValue.value.notificationTime = new Date(notificationTime.value).toISOString()
    await notesStore.createNote(formValue.value)
    message.success('Заметка создана!')
    router.push('/notes')
  } catch {
    message.error('Ошибка создания')
  }
}
</script>

