import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Note, CreateNoteRequest, UpdateNoteRequest } from '@/types'
import { notesApi } from '@/services/api'

export const useNotesStore = defineStore('notes', () => {
  // State
  const notes = ref<Note[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Actions
  async function fetchNotes(): Promise<void> {
    try {
      loading.value = true
      error.value = null
      const fetchedNotes = await notesApi.getAll()
      
      // Отладка: смотрим что пришло с сервера
      console.log('📥 Данные с API:', fetchedNotes)
      console.log('📊 Тип данных:', typeof fetchedNotes)
      console.log('📋 Является массивом:', Array.isArray(fetchedNotes))
      
      // Фильтруем только валидные заметки
      if (Array.isArray(fetchedNotes)) {
        const validNotes = fetchedNotes.filter((note) => note != null && note.id != null)
        console.log('✅ Валидные заметки:', validNotes)
        notes.value = validNotes
      } else {
        console.warn('⚠️ Данные не являются массивом:', fetchedNotes)
        notes.value = []
      }
      
      console.log('💾 Сохранено заметок:', notes.value.length)
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Ошибка загрузки заметок'
      console.error('❌ Error fetching notes:', err)
      notes.value = []
    } finally {
      loading.value = false
    }
  }

  async function createNote(data: CreateNoteRequest): Promise<Note> {
    try {
      loading.value = true
      error.value = null
      const newNote = await notesApi.create(data)
      
      // Добавляем только если заметка валидна
      if (newNote && newNote.id != null) {
        notes.value.push(newNote)
      }
      return newNote
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Ошибка создания заметки'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateNote(data: UpdateNoteRequest): Promise<void> {
    try {
      loading.value = true
      error.value = null
      await notesApi.update(data)

      // Обновляем локальный массив
      const index = notes.value.findIndex((n: Note) => n.id === data.id)
      if (index !== -1) {
        notes.value[index] = { ...notes.value[index], ...data }
      }
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Ошибка обновления заметки'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function deleteNote(id: number): Promise<void> {
    try {
      loading.value = true
      error.value = null
      await notesApi.delete(id)

      // Удаляем из локального массива используя requestAnimationFrame для плавности
      requestAnimationFrame(() => {
        const index = notes.value.findIndex((n: Note) => n && n.id === id)
        if (index !== -1) {
          notes.value.splice(index, 1)
        }
      })
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Ошибка удаления заметки'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function exportToCsv(): Promise<void> {
    try {
      loading.value = true
      error.value = null
      const blob = await notesApi.exportToCsv()
      
      // Создаем ссылку для скачивания
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = `notes_${new Date().toISOString().split('T')[0]}.csv`
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      window.URL.revokeObjectURL(url)
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Ошибка экспорта'
      throw err
    } finally {
      loading.value = false
    }
  }

  function clearError(): void {
    error.value = null
  }

  return {
    // State
    notes,
    loading,
    error,
    // Actions
    fetchNotes,
    createNote,
    updateNote,
    deleteNote,
    exportToCsv,
    clearError
  }
})

